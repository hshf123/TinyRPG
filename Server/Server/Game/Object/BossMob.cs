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

            State = CreatureState.Skill;
            Dir = MoveDir.Down;
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

        // FSM (Finite State Machine)
        public override void Update()
        {
            switch (State)
            {
                case CreatureState.Idle:
                    break;
                case CreatureState.Moving:
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
        //int _searchRange = 7; // 몬스터가 플레이어를 찾는 범위
        //int _chaseRange = 10; // 몬스터가 플레이어를 쫓아가는 범위
        //int _skillRange = 1; // 몬스터가 스킬을 사용하게 되는 범위
        //long _nextSearchTick = 0;
        //protected virtual void UpdateIdle()
        //{
        //    if (_nextSearchTick > Environment.TickCount64)
        //        return;
        //    _nextSearchTick = Environment.TickCount64 + 1000;

        //    _target = Scene.FindPlayer(p =>
        //    {
        //        Vector2Int dir = p.CellPos - CellPos;   // 방향벡터 추출
        //        return dir.cellDist < _searchRange;     // 거리 추출
        //    });

        //    if (_target == null)
        //        return;

        //    State = CreatureState.Moving;
        //}
        //long _nextMoveTick = 0;
        //protected virtual void UpdateMoving()
        //{
        //    if (_nextMoveTick > Environment.TickCount64)
        //        return;
        //    int moveTick = (int)(1000 / Speed);
        //    _nextMoveTick = Environment.TickCount64 + moveTick;

        //    if (_target == null || _target.Scene != Scene)
        //    {
        //        _target = null;
        //        State = CreatureState.Idle;
        //        BroadcastMove();
        //        return;
        //    }

        //    Vector2Int dir = _target.CellPos - CellPos;
        //    int dist = dir.cellDist;
        //    if (dist == 0 || dist > _chaseRange)
        //    {
        //        _target = null;
        //        State = CreatureState.Idle;
        //        BroadcastMove();
        //        return;
        //    }

        //    List<Vector2Int> path = Scene.Map.FindPath(CellPos, _target.CellPos, false);
        //    if (path.Count < 2 || path.Count > _chaseRange)
        //    {
        //        _target = null;
        //        State = CreatureState.Idle;
        //        BroadcastMove();
        //        return;
        //    }

        //    // 스킬로 이동할지 체크
        //    if (dist <= _skillRange && (dir.x == 0 || dir.y == 0))
        //    {
        //        _coolTick = 0;
        //        State = CreatureState.Skill;
        //        return;
        //    }

        //    // 이동
        //    Dir = GetDirFromVec(path[1] - CellPos);
        //    Scene.Map.ApplyMove(this, path[1]);
        //    BroadcastMove();
        //}
        //void BroadcastMove()
        //{
        //    S_Move movePacket = new S_Move();
        //    movePacket.ObjectId = Id;
        //    movePacket.PosInfo = PosInfo;
        //    Scene.Broadcast(movePacket);
        //}
        int _coolTick = 0;
        protected virtual void UpdateSkill()
        {
            if (_coolTick == 0)
            {
                // TODO : 보스 몬스터의 공격 패턴
                // 동서남북 4가지 방향으로 화살을 일정 틱마다 계속해서 발사!
                // 사실상 깨는게 의미가 없긴하지만...
                Skill skillData = null;
                DataManager.SkillDict.TryGetValue(3, out skillData);

                for(int i=0; i<4; i++)
                {
                    Arrow arrow = ObjectManager.Instance.Add<Arrow>();
                    if (arrow == null)
                        return;

                    arrow.Owner = this;
                    arrow.Data = skillData;
                    arrow.PosInfo.State = CreatureState.Moving;
                    arrow.PosInfo.MoveDir = (MoveDir)i;
                    arrow.PosInfo.PosX = PosInfo.PosX;
                    arrow.PosInfo.PosY = PosInfo.PosY;
                    arrow.Speed = skillData.projectile.speed;

                    Scene.EnterGame(arrow);
                }

                //S_Skill skillPacket = new S_Skill() { Info = new SkillInfo() };
                //skillPacket.ObjectId = Id;
                //skillPacket.Info.SkillId = skillData.id;
                //Scene.Broadcast(skillPacket);

                int coolTick = (int)(1000 * skillData.cooldown);
                _coolTick = (int)(Environment.TickCount64 + coolTick);
            }

            if (_coolTick > Environment.TickCount64)
                return;

            _coolTick = 0;
        }
        protected virtual void UpdateDead()
        {

        }
        public override void OnDead(GameObject attacker)
        {
            S_Die diePacket = new S_Die();
            diePacket.ObjectId = Id;
            diePacket.AttackerId = attacker.Id;
            Scene.Broadcast(diePacket);

            Scenes scene = Scene;
            scene.LeaveGame(Id);

            StatInfo.Hp = StatInfo.MaxHp;
            PosInfo.State = CreatureState.Skill;
            PosInfo.MoveDir = MoveDir.Down;
            CellPos = new Vector2Int(0, 0);

            scene.EnterGame(this);
        }
    }
}
