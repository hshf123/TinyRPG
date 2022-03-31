using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public struct Pos
{
    public Pos(int y, int x) { Y = y; X = x; }
    public int Y;
    public int X;
}

public struct PQNode : IComparable<PQNode>
{
    public int F;
    public int G;
    public int Y;
    public int X;

    public int CompareTo(PQNode other)
    {
        if (F == other.F)
            return 0;
        return F < other.F ? 1 : -1;
    }
}

public class MapManager
{
    public Grid CurrentGrid { get; private set; }
    public Dictionary<Vector3Int, SceneType> PortalPos { get; private set; } = new Dictionary<Vector3Int, SceneType>();

    public int MinX { get; set; }
    public int MaxX { get; set; }
    public int MinY { get; set; }
    public int MaxY { get; set; }

    public int SizeX { get { return MaxX - MinX + 1; } }
    public int SizeY { get { return MaxY - MinY + 1; } }

    bool[,] _collision;

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
        return !_collision[y, x]; // collision �迭�� true�� �� �� ���°Ű� false�� �� �� �ִ� ��
    }

    // ��Ż���� üũ
    public bool IsPortal(Vector3Int cellPos)
    {
        // ��Ż���� �ƴ���
        return PortalPos.ContainsKey(cellPos);

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
        //Portal ���� ����
        TextAsset portalText = Managers.Resource.Load<TextAsset>($"Map/{name}_Portal");

        reader = new StringReader(portalText.text);

        int count = int.Parse(reader.ReadLine());
        for (int j = 0; j < count; j++)
        {
            string linkName = reader.ReadLine(); // ���� ���� �Ǿ� �ִ���
            int x = int.Parse(reader.ReadLine());
            int y = int.Parse(reader.ReadLine());
            SceneType scene = SceneType.Unknown;
            switch(linkName)
            {
                case "Lobby":
                    scene = SceneType.Lobby;
                    break;
                case "Huntingground":
                    scene = SceneType.Huntingground;
                    break;
            }
            PortalPos.Add(new Vector3Int(x, y, 0), scene);
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
        PortalPos.Clear();
        Managers.Object.Clear();
    }

    #region A*

    int[] _deltaY = new int[] { 1, -1, 0, 0 };
    int[] _deltaX = new int[] { 0, 0, -1, 1 };
    int[] _cost = new int[] { 10, 10, 10, 10 };

    public List<Vector3Int> FindPath(Vector3Int startCellPos, Vector3Int destCellPos, bool ignoreDestCollision = false/*�浹�� ��������*/)
    {
        List<Pos> path = new List<Pos>();

        // ���� �ű��
        // F = G + H
        // F = ���� ���� (���� ���� ����, ��ο� ���� �޶���)
        // G = ���������� �ش� ��ǥ���� �̵��ϴµ� ��� ��� (���� ���� ����, ��ο� ���� �޶���)
        // H = ���������� �󸶳� ������� (���� ���� ����, ����)

        // (y, x) �̹� �湮�ߴ��� ���� (�湮�� ������ true)
        bool[,] closed = new bool[SizeY, SizeX];

        // (y, x) ���� ���� �� ���̶� �߰��� �ߴ���
        // �߰� X => MaxValue
        // �߰� O => F = G + H
        int[,] open = new int[SizeY, SizeX];
        for (int y = 0; y < SizeY; y++)
            for (int x = 0; x < SizeX; x++)
                open[y, x] = Int32.MaxValue;

        Pos[,] parent = new Pos[SizeY, SizeX];

        // ���¸���Ʈ�� �ִ� ������ �߿���, ���� ���� �ĺ��� ������ �̾ƿ��� ���� PriorityQueue�� ����Ѵ�.
        PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>();

        // CellPos -> ArrayPos;
        Pos pos = Cell2Pos(startCellPos);
        Pos dest = Cell2Pos(destCellPos);

        // ������ �߰�
        open[pos.Y, pos.X] = 10 * (Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X));
        pq.Push(new PQNode() { F = 10 * (Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X)), G = 0, Y = pos.Y, X = pos.X });
        parent[pos.Y, pos.X] = new Pos(pos.Y, pos.X);

        while(pq.Count > 0)
        {
            // ���� ���� �ĺ��� ã�´�
            PQNode node = pq.Pop();
            // ������ ��ǥ�� ���� ��η� ã�Ƽ�, �� ���� ��η� ���ؼ� �̹� �湮�� ��� ��ŵ
            if (closed[node.Y, node.X])
                continue;

            // �湮�Ѵ�
            closed[node.Y, node.X] = true;
            // �������� ���������� �ٷ� ����
            if (node.Y == dest.Y && node.X == dest.X)
                break;

            // �����¿� �� �̵��� �� �ִ� ��ǥ���� Ȯ���ؼ� ����(open)�Ѵ�
            for (int i = 0; i < _deltaY.Length; i++)
            {
                Pos next = new Pos(node.Y + _deltaY[i], node.X + _deltaX[i]);

                // ��ȿ ������ ������� ��ŵ
                // ������ ������ �� �� ������ ��ŵ
                if (!ignoreDestCollision || next.Y != dest.Y || next.X != dest.X)
                {
                    if (CanGo(Pos2Cell(next)) == false) // CellPos
                        continue;
                }

                // �̹� �湮�� ���̸� ��ŵ
                if (closed[next.Y, next.X])
                    continue;

                // ��� ���
                int g = 0;// node.G + _cost[i];
                int h = 10 * ((dest.Y - next.Y) * (dest.Y - next.Y) + (dest.X - next.X) * (dest.X - next.X));
                // �ٸ� ��ο��� �� ���� �� �̹� ã������ ��ŵ
                if (open[next.Y, next.X] < g + h)
                    continue;

                // ���� ����
                open[dest.Y, dest.X] = g + h;
                pq.Push(new PQNode() { F = g + h, G = g, Y = next.Y, X = next.X });
                parent[next.Y, next.X] = new Pos(node.Y, node.X);
            }
        }

        return CalcCellPathFromParent(parent, dest);
    }

    List<Vector3Int> CalcCellPathFromParent(Pos[,] parent, Pos dest)
    {
        List<Vector3Int> cells = new List<Vector3Int>();

        int y = dest.Y;
        int x = dest.X;
        while(parent[y,x].Y!=y||parent[y,x].X!=x)
        {
            cells.Add(Pos2Cell(new Pos(y, x)));
            Pos pos = parent[y, x];
            y = pos.Y;
            x = pos.X;
        }
        cells.Add(Pos2Cell(new Pos(y, x)));
        cells.Reverse();

        return cells;
    }

    Pos Cell2Pos(Vector3Int cell)
    {
        return new Pos(MaxY - cell.y, cell.x - MinX);
    }

    Vector3Int Pos2Cell(Pos pos)
    {
        return new Vector3Int(pos.X + MinX, MaxY - pos.Y, 0);
    }
    #endregion
}
