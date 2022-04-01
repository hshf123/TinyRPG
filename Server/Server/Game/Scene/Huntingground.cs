using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    class Huntingground : Scenes
    {
        public Huntingground()
        {
            //sceneType = SceneType.Huntingground;
        }

        public override void Init()
        {
            Map.LoadMap("Huntingground");

            // TEMP
            Monster monster = ObjectManager.Instance.Add<Monster>();
            monster.CellPos = new Vector2Int(-5, -5);
            EnterGame(monster);
        }

        public void MonsterSpawn()
        {

        }
    }
}
