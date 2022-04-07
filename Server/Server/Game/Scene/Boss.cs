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
            Game.BossMob boss = ObjectManager.Instance.Add<Game.BossMob>();

            boss.CellPos = new Vector2Int(0, 0);
            Push(EnterGame, boss);
        }
    }
}
