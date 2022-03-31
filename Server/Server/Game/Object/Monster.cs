using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    class Monster : GameObject
    {
        public Monster()
        {
            ObjectType = GameObjectType.Monster;

            // TODO : DataSheet로 따로 빼서 관리

            StatInfo.Level = 1;
            StatInfo.Hp = 100;
            StatInfo.MaxHp = 100;
            StatInfo.Speed = 5.0f;

            State = CreatureState.Idle;
        }

        // FSM (Finite State Machine)
        public override void Update()
        {
            switch(State)
            {
                case CreatureState.Idle:
                    UpdateIdle();
                    break;
                case CreatureState.Moving:
                    UpdateMoving();
                    break;
                case CreatureState.Skill:
                    UpdateSkill();
                    break;
                case CreatureState.Dead:
                    UpdateDead();
                    break;
            }
        }

        Player _target;
        int _searchRange = 10; // 몬스터가 플레이어를 찾는 범위
        int _chaseRange = 10; // 몬스터가 플레이어를 쫓아가는 범위
        long _nextSearchTick = 0;
        protected virtual void UpdateIdle()
        {
            if (_nextSearchTick > Environment.TickCount64)
                return;
            _nextSearchTick = Environment.TickCount64 + 1000;

            _target = Scene.FindPlayer(p =>
            {
                Vector2Int dir = p.CellPos - CellPos;   // 방향벡터 추출
                return dir.cellDist < _searchRange;     // 거리 추출
            });

            if (_target == null)
                return;

            State = CreatureState.Moving;
        }
        long _nextMoveTick = 0;
        protected virtual void UpdateMoving()
        {
            if (_nextMoveTick > Environment.TickCount64)
                return;
            int moveTick = (int)(1000 / Speed);
            _nextMoveTick = Environment.TickCount64 + moveTick;

            if(_target == null || _target.Scene != Scene    )
            {
                _target = null;
                State = CreatureState.Idle;
                return;
            }

            int dist = (_target.CellPos - CellPos).cellDist;
            if (dist == 0 || dist > _chaseRange)
            {
                _target = null;
                State = CreatureState.Idle;
                return;
            }

            List<Vector2Int> path = Scene.Map.FindPath(CellPos, _target.CellPos, false);
            if (path.Count < 2 || path.Count > _chaseRange)
            {
                _target = null;
                State = CreatureState.Idle;
                return;
            }

            // 이동
            Dir = GetDirFromVec(path[1] - CellPos);
            Scene.Map.ApplyMove(this, path[1]);

            // 다른 플레이어한테도 알려준다
            S_Move movePacket = new S_Move();
            movePacket.ObjectId = Id;
            movePacket.PosInfo = PosInfo;
            Scene.Broadcast(movePacket);
        }
        protected virtual void UpdateSkill()
        {
        }
        protected virtual void UpdateDead()
        {

        }
    }
}
