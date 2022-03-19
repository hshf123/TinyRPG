using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapManager
{
    public Grid CurrentGrid { get; private set; }
    public List<Vector3Int> PortalPos { get; private set; } = new List<Vector3Int>();

    public int MinX { get; set; }
    public int MaxX { get; set; }
    public int MinY { get; set; }
    public int MaxY { get; set; }

    bool[,] _collision;
    bool[,] _portal;

    // �� �� �ִ� �������� üũ
    public bool CanGo(Vector3Int cellPos)
    {
        // �ִ� ���� üũ
        if (cellPos.x < MinX || cellPos.x > MaxX)
            return false;
        if (cellPos.y < MinY || cellPos.y > MaxY)
            return false;

        // �浹 ���� üũ
        int x = cellPos.x - MinX;
        int y = MaxY - cellPos.y;
        return !_collision[y,x]; // collision �迭�� true�� �� �� ���°Ű� false�� �� �� �ִ� ��
    }

    // ��Ż���� üũ
    public bool IsPortal(Vector3Int cellPos)
    {
        // ��Ż���� �ƴ���
        int x = cellPos.x - MinX;
        int y = MaxY - cellPos.y;
        return _portal[y, x]; // portal �迭�� true�� ��Ż false�� ��
    }

    public void LoadMap(string name)
    {
        DestroyMap(name);

        string mapName = "Map_" + name;
        GameObject go = Managers.Resource.Instantiate($"Map/{mapName}");
        go.name = mapName;

        GameObject collision = Util.FindChild(go, "Collision", true);
        if (collision != null)
            collision.SetActive(false);
        GameObject portal = Util.FindChild(go, "Portal", true);
        if (portal != null)
            portal.SetActive(false);

        CurrentGrid = go.GetComponent<Grid>();

        // Collision ���� ����
        TextAsset collisionText = Managers.Resource.Load<TextAsset>($"Map/{name}_Collision");
        StringReader reader = new StringReader(collisionText.text);
        MinX = int.Parse(reader.ReadLine());
        MaxX = int.Parse(reader.ReadLine());
        MinY = int.Parse(reader.ReadLine());
        MaxY = int.Parse(reader.ReadLine());
        int xCount = MaxX - MinX + 1;
        int yCount = MaxY - MinY + 1;
        _collision = new bool[yCount, xCount];
        for (int y = 0; y < yCount; y++)
        {
            string line = reader.ReadLine();
            for (int x = 0; x < xCount; x++)
            {
                _collision[y, x] = (line[x] == '1' ? true : false);
            }
        }
        // Portal ���� ����
        TextAsset portalText = Managers.Resource.Load<TextAsset>($"Map/{name}_Portal");
        reader = new StringReader(portalText.text);
        MinX = int.Parse(reader.ReadLine());
        MaxX = int.Parse(reader.ReadLine());
        MinY = int.Parse(reader.ReadLine());
        MaxY = int.Parse(reader.ReadLine());
        xCount = MaxX - MinX + 1;
        yCount = MaxY - MinY + 1;
        _portal = new bool[yCount, xCount];
        for (int y = 0; y < yCount; y++)
        {
            string line = reader.ReadLine();
            for (int x = 0; x < xCount; x++)
            {
                //_portal[y, x] = (line[x] == '1' ? true : false);
                if (line[x] == '1')
                {
                    _portal[y, x] = true;
                    PortalPos.Add(new Vector3Int(x + MinX, y - MaxY, 0));
                }
                else
                {
                    _portal[y, x] = false;
                }
            }
        }
    }

    public void DestroyMap(string name)
    {
        GameObject map = GameObject.Find($"Map_{name}");
        if (map != null)
        {
            GameObject.Destroy(map);
            CurrentGrid = null;
        }
    }
}
