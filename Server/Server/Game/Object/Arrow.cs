using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    class Arrow : Projectile
    {
        public GameObject Owner { get; set; }

        long _nextMoveTick = 0;
        int _crossroad = 0; // 화살 사거리 체크

        public override void Update()
        {
            if (Data == null || Owner == null || Scene == null || Data.projectile == null)
                return;

            // 프레임 계산
            if (_nextMoveTick >= Environment.TickCount64)
                return;

            long tick = (long)(1000 / Data.projectile.speed);
            _nextMoveTick = Environment.TickCount64 + tick;

            // 화살이 앞으로 나가는 연산
            Vector2Int destPos = GetFrontCellPos();
            if (Scene.Map.CanGo(destPos) && _crossroad <= Data.projectile.range)
            {
                CellPos = destPos;
                _crossroad++;

                S_Move movePacket = new S_Move();
                movePacket.ObjectId = Id;
                movePacket.PosInfo = PosInfo;
                Scene.Broadcast(movePacket);

                Console.WriteLine("Move Arrow");
            }
            else
            {
                GameObject target = Scene.Map.Find(destPos);
                if (target != null)
                {
                    // 피격판정
                    target.OnDamaged(this, Data.damage);
                }

                // 소멸
                Scene.LeaveGame(Id);
            }
        }
    }
}
