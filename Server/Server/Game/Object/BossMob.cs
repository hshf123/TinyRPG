using Google.Protobuf.Protocol;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    class BossMob : GameObject
    {
        public BossMob()
        {
            ObjectType = GameObjectType.BossMob;

            StatInfo stat = null;
            if (DataManager.BossStatDict.TryGetValue(1, out stat) == false)
                return;

            StatInfo.Level = stat.Level;
            StatInfo.Hp = stat.Hp;
            StatInfo.MaxHp = stat.MaxHp;
            StatInfo.Attack = stat.Attack;
            StatInfo.Speed = stat.Speed;

            State = CreatureState.Idle;
        }
    }
}
