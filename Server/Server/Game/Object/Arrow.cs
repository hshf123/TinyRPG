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
        int _crossroad = 0; // 화살 사거리

        public override void Update()
        {
            if (Owner == null || Scene == null)
                return;

            // 프레임 계산
            if (_nextMoveTick >= Environment.TickCount64)
                return;
            _nextMoveTick = Environment.TickCount64 + 50;

            // 화살이 앞으로 나가는 연산
            Vector2Int destPos = GetFrontCellPos();
            if (Scene.Map.CanGo(destPos) && _crossroad <= 8)
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
                    // TODO : 피격 판정
                }

                // 소멸
                Scene.LeaveGame(Id);
            }
        }
    }
}
