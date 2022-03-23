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
            MyPlayer.PosInfo = info.PosInfo;
        }
        else
        {
            GameObject player = Managers.Resource.Instantiate("Creature/Player");
            player.name = info.Name;
            _objects.Add(info.PlayerId, player);

            PlayerController pc = player.GetComponent<PlayerController>();
            pc.Id = info.PlayerId;
            pc.PosInfo = info.PosInfo;
        }
    }

    public void Remove(int id)
    {
        GameObject go = Find(id);
        if (go == null)
            return;
        Managers.Resource.Destroy(go);
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
        foreach (GameObject go in _objects.Values)
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

    public GameObject Find(int id)
    {
        GameObject go = null;
        if (_objects.TryGetValue(id, out go))
            return go;

        return null;
    }

    public void Clear()
    {
        foreach(GameObject go in _objects.Values)
            Managers.Resource.Destroy(go);
        _objects.Clear();
    }
}
