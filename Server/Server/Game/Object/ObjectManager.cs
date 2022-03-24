using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class ObjectManager
    {
        public static ObjectManager Instance { get; } = new ObjectManager();

        object _lock = new object();
        Dictionary<int, Player> _players = new Dictionary<int, Player>();

        // Bit Flag
        // [부호(1)타입(7)][ID(24)]
        int _typeId = 0;

        public T Add<T>() where T : GameObject, new()
        {
            T obj = new T();
            lock (_lock)
            {
                obj.Id = GenerateId(obj.ObjectType);

                if(obj.ObjectType == GameObjectType.Player)
                {
                    _players.Add(obj.Id, obj as Player);
                }

            }
            return obj;
        }

        int GenerateId(GameObjectType type)
        {
            return ((int)type << 24 | _typeId++);
        }

        public static GameObjectType GetObjectTypeById(int id)
        {
            // id를 24비트 끌어내리고 0x7F는 127을 의미 &을 하면 7자리 빼고 전부 다 0으로 밀 수 있음
            return (GameObjectType)((id >> 24) & 0x7F);
        }

        public bool Remove(int objectId)
        {
            GameObjectType type = GetObjectTypeById(objectId);

            lock (_lock)
            {
                if (type == GameObjectType.Player)
                    return _players.Remove(objectId);
            }

            return false;
        }

        public Player Find(int objectId)
        {
            GameObjectType type = GetObjectTypeById(objectId);

            lock (_lock)
            {
                if(type==GameObjectType.Player)
                {
                    Player player = null;
                    if (_players.TryGetValue(objectId, out player))
                        return player;
                }
            }

            return null;
        }
    }
}
