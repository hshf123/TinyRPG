using Google.Protobuf;
using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class Scenes
    {
        public int SceneId { get; set; }

        object _lock = new object();

        //protected SceneType sceneType { get; set; } = SceneType.Unknown;
        List<Player> _players = new List<Player>();

        public void EnterGame(Player newPlayer)
        {
            if (newPlayer == null)
                return;

            lock (_lock)
            {
                _players.Add(newPlayer);
                newPlayer.Scene = this;

                // 본인한테 정보 전송
                {
                    // 방에 입장 했다는 것을 알려줌
                    S_EnterGame enterPacket = new S_EnterGame();
                    enterPacket.Player = newPlayer.Info;
                    newPlayer.Session.Send(enterPacket);

                    // 원래 방에 있던 애들에 대한 정보를 전송
                    S_Spawn spawnPacket = new S_Spawn();
                    foreach (Player p in _players)
                    {
                        if (p != newPlayer)
                            spawnPacket.Players.Add(p.Info);
                    }
                    newPlayer.Session.Send(spawnPacket);
                }
                // 타인들한테 정보 전송
                {
                    S_Spawn spawnPacket = new S_Spawn();
                    spawnPacket.Players.Add(newPlayer.Info);
                    foreach (Player p in _players)
                    {
                        if (p != newPlayer)
                            p.Session.Send(spawnPacket);
                    }
                }
            }
        }

        public void LeaveGame(int playerId)
        {
            Player player = _players.Find(p => p.Info.PlayerId == playerId);
            if (player == null)
                return;

            _players.Remove(player);
            player.Scene = null;

            // 본인한테 정보 전송
            {
                // 방에서 나갔다는 것을 알려줌
                S_LeaveGame leavePacket = new S_LeaveGame();
                player.Session.Send(leavePacket);
            }
            // 타인들한테 정보 전송
            {
                S_Despawn despawnPacket = new S_Despawn();
                despawnPacket.PlayerIds.Add(player.Info.PlayerId);
                foreach (Player p in _players)
                {
                    if (p != player)
                        p.Session.Send(despawnPacket);
                }
            }
        }

        public void HandleMove(Player player, C_Move packet)
        {
            if (player == null)
                return;
            lock (_lock)
            {
                // TODO : 검증 (클라가 해킹되어서 잘못된 정보로 들어올 수도 있으므로)

                // 일단 서버에서 좌표이동(아직 클라에서는 이동X)
                PlayerInfo info = player.Info;
                info.PosInfo = packet.PosInfo;

                // 플레이어가 속한 씬에있는 모든 유저에게 브로드캐스트
                S_Move resMovePacket = new S_Move();
                resMovePacket.PlayerId = info.PlayerId;
                resMovePacket.PosInfo = info.PosInfo;
                Broadcast(resMovePacket);
            }
        }

        public void HandleSkill(Player player, C_Skill packet)
        {
            if (player == null)
                return;

            lock (_lock)
            {
                PlayerInfo info = player.Info;
                if (info.PosInfo.State != CreatureState.Idle)
                    return;

                // TODO : 스킬 사용 가능 여부 체크

                // 통과
                info.PosInfo.State = CreatureState.Skill;
                S_Skill skillPacket = new S_Skill() { Info = new SkillInfo() };
                skillPacket.PlayerId = player.Info.PlayerId;
                skillPacket.Info.SkillId = 1;
                Broadcast(skillPacket);

                // TODO : 데미지 판정
            }
        }

        // 속해있는 씬에 있는 모두에게 브로드 캐스트
        public void Broadcast(IMessage packet)
        { 
            lock(_lock)
            {
                foreach(Player p in _players)
                {
                    p.Session.Send(packet);
                }
            }
        }
    }
}
