using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
    public MyPlayerController MyPlayer { get; set; } // 나를 찾기위한 용도
    Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>(); // 서버 연동 대비

    public static GameObjectType GetObjectTypeById(int id)
    {
        // id를 24비트 끌어내리고 0x7F는 127을 의미 &을 하면 7자리 빼고 전부 다 0으로 밀 수 있음
        return (GameObjectType)((id >> 24) & 0x7F);
    }

    public void Add(ObjectInfo info, bool myPlayer = false)
    {
        GameObjectType type = GetObjectTypeById(info.ObjectId);
        if(type == GameObjectType.Player)
        {
            if (myPlayer)
            {
                GameObject player = Managers.Resource.Instantiate("Creature/MyPlayer");
                player.name = info.Name;
                _objects.Add(info.ObjectId, player);

                MyPlayer = player.GetComponent<MyPlayerController>();
                MyPlayer.Id = info.ObjectId;
                MyPlayer.PosInfo = info.PosInfo;
                MyPlayer.Stat = info.StatInfo;
                MyPlayer.SyncPos();
            }
            else
            {
                GameObject player = Managers.Resource.Instantiate("Creature/Player");
                player.name = info.Name;
                _objects.Add(info.ObjectId, player);

                PlayerController pc = player.GetComponent<PlayerController>();
                pc.Id = info.ObjectId;
                pc.PosInfo = info.PosInfo;
                pc.Stat = info.StatInfo;
                pc.SyncPos();
            }
        }
        else if(type == GameObjectType.Monster)
        {
            // TODO
        }
        else if(type == GameObjectType.Projectile)
        {
            GameObject go = Managers.Resource.Instantiate("Projectile/Arrow");
            go.name = "Arrow";
            _objects.Add(info.ObjectId, go);

            ArrowController ac = go.GetComponent<ArrowController>();
            ac.PosInfo = info.PosInfo;
            ac.Stat = info.StatInfo;
            ac.SyncPos();
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

    public GameObject FindCreature(Vector3Int cellPos)
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
        MyPlayer = null;
    }
}
