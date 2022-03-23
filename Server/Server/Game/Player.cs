using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class Player
    {
        public PlayerInfo Info { get; set; } = new PlayerInfo() { PosInfo = new PositionInfo() };
        public Scenes Scene { get; set; } // 플레이어가 어떤 씬에 있는지
        public ClientSession Session { get; set; }
    }
}
