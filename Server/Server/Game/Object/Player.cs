using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class Player : GameObject
    {
        public ClientSession Session { get; set; }

        public Player()
        {
            ObjectType = GameObjectType.Player;
        }

        public override void OnDamaged(GameObject attacker, int damage)
        {
            StatInfo.Hp = Math.Max(StatInfo.Hp - damage, 0);

            S_ChangeHp hpPacket = new S_ChangeHp();
            hpPacket.ObjectId = Id;
            hpPacket.Hp = StatInfo.Hp;
            Scene.Broadcast(hpPacket);

            if (StatInfo.Hp <= 0)
                OnDead(attacker);
        }
    }
}
