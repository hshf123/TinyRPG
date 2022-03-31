using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class GameObject
    {
        public int Id { get { return Info.ObjectId; } set { Info.ObjectId = value; } }
        public GameObjectType ObjectType { get; protected set; } = GameObjectType.None;
        public Scenes Scene { get; set; } // 오브젝트가 어떤 씬에 있는지
        public ObjectInfo Info { get; set; } = new ObjectInfo();
        public PositionInfo PosInfo { get; private set; } = new PositionInfo();
        public CreatureState State { get { return PosInfo.State; } set { PosInfo.State = value; } }
        public MoveDir Dir { get { return PosInfo.MoveDir; } set { PosInfo.MoveDir = value; } }
        public StatInfo StatInfo { get; private set; } = new StatInfo();
        public float Speed { get { return StatInfo.Speed; } set { StatInfo.Speed = value; } }
        public int Hp { get { return StatInfo.Hp; } set { StatInfo.Hp = Math.Clamp(value, 0, StatInfo.MaxHp); } }

        public GameObject()
        {
            Info.PosInfo = PosInfo;
            Info.StatInfo = StatInfo;
        }

        public virtual void Update()
        {

        }

        public Vector2Int CellPos
        {
            get { return new Vector2Int(PosInfo.PosX, PosInfo.PosY); }
            set
            {
                PosInfo.PosX = value.x;
                PosInfo.PosY = value.y;
            }
        }

        public Vector2Int GetFrontCellPos()
        {
            return GetFrontCellPos(PosInfo.MoveDir);
        }

        public static MoveDir GetDirFromVec(Vector2Int dir)
        {
            if (dir.x > 0)
                return MoveDir.Right;
            else if (dir.x < 0)
                return MoveDir.Left;
            else if (dir.y > 0)
                return MoveDir.Up;
            else
                return MoveDir.Down;
        }

        public Vector2Int GetFrontCellPos(MoveDir dir)
        {
            Vector2Int cellPos = CellPos;

            switch (dir)
            {
                case MoveDir.Up:
                    cellPos += Vector2Int.up;
                    break;
                case MoveDir.Right:
                    cellPos += Vector2Int.right;
                    break;
                case MoveDir.Down:
                    cellPos += Vector2Int.down;
                    break;
                case MoveDir.Left:
                    cellPos += Vector2Int.left;
                    break;
            }

            return cellPos;
        }

        public virtual void OnDamaged(GameObject attacker, int damage)
        {
            
        }

        public virtual void OnDead(GameObject attacker)
        {
            S_Die diePacket = new S_Die();
            diePacket.ObjectId = Id;
            diePacket.AttackerId = attacker.Id;
            Scene.Broadcast(diePacket);

            Scenes scene = Scene;
            scene.LeaveGame(Id);

            StatInfo.Hp = StatInfo.MaxHp;
            PosInfo.State = CreatureState.Idle;
            PosInfo.MoveDir = MoveDir.Down;
            PosInfo.PosX = 0;
            PosInfo.PosY = 0;

            scene.EnterGame(this);
        }
    }
}
