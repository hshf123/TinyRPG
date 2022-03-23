using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
    public MyPlayerController MyPlayer { get; set; } // 나를 찾기위한 용도
    Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>(); // 서버 연동 대비

    public void Add(PlayerInfo info, bool myPlayer = false)
    {
        if (myPlayer)
        {
            GameObject player = Managers.Resource.Instantiate("Creature/MyPlayer");
            player.name = info.Name;
            _objects.Add(info.PlayerId, player);

            MyPlayer = player.GetComponent<MyPlayerController>();
            MyPlayer.Id = info.PlayerId;
            MyPlayer.CellPos = new Vector3Int(info.PosX, info.PosY, 0);
        }
        else
        {
            GameObject player = Managers.Resource.Instantiate("Creature/Player");
            player.name = info.Name;
            _objects.Add(info.PlayerId, player);

            PlayerController pc = player.GetComponent<MyPlayerController>();
            pc.Id = info.PlayerId;
            pc.CellPos = new Vector3Int(info.PosX, info.PosY, 0);
        }
    }

    public void Add(int id, GameObject go)
    {
        _objects.Add(id, go);
    }

    public void Remove(int id)
    {
        _objects.Remove(id);
    }

    public void RemoveMyPlayer()
    {
        if (MyPlayer == null)
            return;

        Remove(MyPlayer.Id);
        MyPlayer = null;
    }

    public GameObject Find(Vector3Int cellPos)
    {
        foreach(GameObject go in _objects.Values)
        {
            CreatureController cc = go.GetComponent<CreatureController>();
            if (cc == null)
                continue;

            if (cc.CellPos == cellPos)
                return go;
        }

        return null;
    }

    public GameObject Find(Func<GameObject, bool> condition)
    {
        foreach (GameObject go in _objects.Values)
        {
            if (condition.Invoke(go))
                return go;
        }

        return null;
    }

    public void Clear()
    {
        _objects.Clear();
    }
}
