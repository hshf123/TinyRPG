using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game.Scene
{
    class Boss : Scenes
    {
        public override void Init()
        {
            Map.LoadMap("Boss");

            BossSpawn();
        }

        void BossSpawn()
        {
            // TODO : 보스 스폰
        }
    }
}
