using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Data;
using Server.Game;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    abstract public class Scenes : JobSerializer
    {
        public int SceneId { get; set; }

        Dictionary<int, Player> _players = new Dictionary<int, Player>();
        Dictionary<int, Monster> _monsters = new Dictionary<int, Monster>();
        Dictionary<int, Projectile> _projectiles = new Dictionary<int, Projectile>();
        Dictionary<int, BossMob> _bossMobs = new Dictionary<int, BossMob>();

        public Map Map { get; private set; } = new Map();

        public abstract void Init();

        public void Update()
        {
            foreach (Projectile projectile in _projectiles.Values)
            {
                projectile.Update();
            }

            foreach (Monster monster in _monsters.Values)
            {
                monster.Update();
            }

            Flush();
        }

        public void EnterGame(GameObject newObject)
        {
            if (newObject == null)
                return;

            GameObjectType type = ObjectManager.GetObjectTypeById(newObject.Id);

            if (type == GameObjectType.Player)
            {
                Player player = newObject as Player;
                _players.Add(newObject.Id, player);
                player.Scene = this;

                Map.ApplyMove(player, new Vector2Int(player.PosInfo.PosX, player.PosInfo.PosY));

                // 본인한테 정보 전송
                {
                    // 방에 입장 했다는 것을 알려줌
                    S_EnterGame enterPacket = new S_EnterGame();
                    enterPacket.Player = newObject.Info;
                    player.Session.Send(enterPacket);

                    // 원래 방에 있던 애들에 대한 정보를 전송
                    S_Spawn spawnPacket = new S_Spawn();
                    foreach (Player p in _players.Values)
                    {
                        if (p != newObject)
                            spawnPacket.Objects.Add(p.Info);
                    }

                    foreach (Monster m in _monsters.Values)
                        spawnPacket.Objects.Add(m.Info);

                    foreach (Projectile p in _projectiles.Values)
                        spawnPacket.Objects.Add(p.Info);

                    foreach (BossMob b in _bossMobs.Values)
                        spawnPacket.Objects.Add(b.Info);

                    player.Session.Send(spawnPacket);
                }
            }
            else if (type == GameObjectType.Monster)
            {
                Monster monster = newObject as Monster;
                _monsters.Add(newObject.Id, monster);
                monster.Scene = this;

                Map.ApplyMove(monster, new Vector2Int(monster.PosInfo.PosX, monster.PosInfo.PosY));
            }
            else if (type == GameObjectType.Projectile)
            {
                Projectile projectile = newObject as Projectile;
                _projectiles.Add(newObject.Id, projectile);
                projectile.Scene = this;
            }
            else if (type == GameObjectType.BossMob)
            {
                BossMob bossMob = newObject as BossMob;
                _bossMobs.Add(newObject.Id, bossMob);
                bossMob.Scene = this;

                Map.ApplyMove(bossMob, new Vector2Int(bossMob.PosInfo.PosX, bossMob.PosInfo.PosY));
            }

            // 타인들한테 정보 전송
            {
                S_Spawn spawnPacket = new S_Spawn();
                spawnPacket.Objects.Add(newObject.Info);
                foreach (Player p in _players.Values)
                    if (p.Id != newObject.Id)
                        p.Session.Send(spawnPacket);
            }
        }

        public void LeaveGame(int objectId)
        {
            GameObjectType type = ObjectManager.GetObjectTypeById(objectId);

            if (type == GameObjectType.Player)
            {
                Player player = null;
                if (_players.Remove(objectId, out player) == false)
                    return;

                Map.ApplyLeave(player);
                player.Scene = null;

                // 본인한테 정보 전송
                {
                    // 방에서 나갔다는 것을 알려줌
                    S_LeaveGame leavePacket = new S_LeaveGame();
                    player.Session.Send(leavePacket);
                }
            }
            else if (type == GameObjectType.Monster)
            {
                Monster monster = null;
                if (_monsters.Remove(objectId, out monster) == false)
                    return;
                Map.ApplyLeave(monster);
                monster.Scene = null;
            }
            else if (type == GameObjectType.Projectile)
            {
                Projectile projectile = null;
                if (_projectiles.Remove(objectId, out projectile) == false)
                    return;
                projectile.Scene = null;
            }
            else if (type == GameObjectType.BossMob)
            {
                BossMob boss = null;
                if (_bossMobs.Remove(objectId, out boss) == false)
                    return;
                Map.ApplyLeave(boss);
                boss.Scene = null;
            }

            // 타인들한테 정보 전송
            {
                S_Despawn despawnPacket = new S_Despawn();
                despawnPacket.ObjectIds.Add(objectId);
                foreach (Player p in _players.Values)
                {
                    if (p.Id != objectId)
                        p.Session.Send(despawnPacket);
                }
            }
        }

        public void HandleMove(Player player, C_Move packet)
        {
            if (player == null)
                return;
            // 검증 (클라가 해킹되어서 잘못된 정보로 들어올 수도 있으므로)
            PositionInfo hopePosition = packet.PosInfo; // 이동을 원하는 좌표
            ObjectInfo info = player.Info;              // 실제 플레이어 좌표

            // PositionInfo안에는 State나 Dir도 담겨있고 이때도 패킷을 전송하니까
            // 좌표가 변경될 때만 체크해주도록
            if (hopePosition.PosX != info.PosInfo.PosX || hopePosition.PosY != info.PosInfo.PosY)
            {
                // 해당 좌표로 이동할 수 있는지 체크해준다.
                if (Map.CanGo(new Vector2Int(hopePosition.PosX, hopePosition.PosY)) == false)
                    return;
            }

            info.PosInfo.State = hopePosition.State;
            info.PosInfo.MoveDir = hopePosition.MoveDir;
            Map.ApplyMove(player, new Vector2Int(hopePosition.PosX, hopePosition.PosY));

            // 플레이어가 속한 씬에있는 모든 유저에게 브로드캐스트
            S_Move resMovePacket = new S_Move();
            resMovePacket.ObjectId = info.ObjectId;
            resMovePacket.PosInfo = info.PosInfo;
            Broadcast(resMovePacket);
        }

        public void HandleSkill(Player player, C_Skill packet)
        {
            if (player == null)
                return;

            ObjectInfo info = player.Info;
            if (info.PosInfo.State != CreatureState.Idle)
                return;

            // 통과
            info.PosInfo.State = CreatureState.Skill;
            S_Skill skillPacket = new S_Skill() { Info = new SkillInfo() };
            skillPacket.ObjectId = player.Info.ObjectId;
            skillPacket.Info.SkillId = packet.Info.SkillId;
            Broadcast(skillPacket);

            Data.Skill skillData = null;
            if (DataManager.SkillDict.TryGetValue(packet.Info.SkillId, out skillData) == false)
                return;

            // TODO : 스킬 사용 가능 여부 체크
            // 스킬 아이디에 따라서 분기 해주기
            if (skillData.skillType == SkillType.SkillAuto) // 평타
            {
                Vector2Int enemyPos = player.GetFrontCellPos(info.PosInfo.MoveDir);
                GameObject target = Map.Find(enemyPos);
                if (target != null)
                {
                    target.OnDamaged(player, player.Info.StatInfo.Attack);
                }
            }
            else if (skillData.skillType == SkillType.SkillProjectile) // 화살 스킬
            {
                Arrow arrow = ObjectManager.Instance.Add<Arrow>();
                if (arrow == null)
                    return;

                arrow.Owner = player;
                arrow.Data = skillData;
                arrow.PosInfo.State = CreatureState.Moving;
                arrow.PosInfo.MoveDir = player.PosInfo.MoveDir;
                arrow.PosInfo.PosX = player.PosInfo.PosX;
                arrow.PosInfo.PosY = player.PosInfo.PosY;
                arrow.Speed = skillData.projectile.speed;
                EnterGame(arrow);
            }
        }

        public void LoadScene(Player player, C_PortalLoad packet)
        {
            SceneType currunt = packet.CurruntScene;
            // TODO : 현재씬에 대한 검증, 이동할 씬에 대한 조건체크
            // 이동가능한지 등등

            SceneType hope = packet.HopeScene;
            S_PortalLoad loadPacket = new S_PortalLoad();
            loadPacket.PlayerId = player.Id;
            loadPacket.HopeScene = hope;
            player.Session.Send(loadPacket);
        }

        public void PortalScene(Player player, C_Portal packet)
        {
            if (player == null || player.Scene == null)
                return;

            SceneType scene = packet.HopeScene;

            // TODO : 포탈위치 데이터시트로 빼기 일단은 0, 0으로
            switch (scene)
            {
                case SceneType.Lobby:
                    player.Scene.LeaveGame(player.Id);

                    player.StatInfo.Hp = player.StatInfo.MaxHp;
                    player.PosInfo.State = CreatureState.Idle;
                    player.PosInfo.MoveDir = MoveDir.Down;
                    player.PosInfo.PosX = 0;
                    player.PosInfo.PosY = 0;

                    SceneManager.Instance.Find(1).EnterGame(player);
                    break;

                case SceneType.Huntingground:
                    player.Scene.LeaveGame(player.Id);

                    player.StatInfo.Hp = player.StatInfo.MaxHp;
                    player.PosInfo.State = CreatureState.Idle;
                    player.PosInfo.MoveDir = MoveDir.Down;
                    player.PosInfo.PosX = 0;
                    player.PosInfo.PosY = 0;

                    SceneManager.Instance.Find(2).EnterGame(player);
                    break;
                case SceneType.Boss:
                    player.Scene.LeaveGame(player.Id);

                    player.StatInfo.Hp = player.StatInfo.MaxHp;
                    player.PosInfo.State = CreatureState.Idle;
                    player.PosInfo.MoveDir = MoveDir.Down;
                    player.PosInfo.PosX = 0;
                    player.PosInfo.PosY = -9;

                    SceneManager.Instance.Find(3).EnterGame(player);
                    break;
            }
        }

        // FindPlayer와 Broadcast 모두 lock 을 잡아주는 부분에서 실행되고 있기 때문에 push를 해줄 필요가 없다.
        // TODO : 체크
        public Player FindPlayer(Predicate<GameObject> condition)
        {
            foreach (Player player in _players.Values)
            {
                if (condition.Invoke(player))
                    return player;
            }

            return null;
        }

        // 속해있는 씬에 있는 모두에게 브로드 캐스트
        public void Broadcast(IMessage packet)
        {
            foreach (Player p in _players.Values)
            {
                p.Session.Send(packet);
            }
        }
    }
}
