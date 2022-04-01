using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    class Huntingground : Scenes
    {
        List<Monster> monsters = new List<Monster>();

        public Huntingground()
        {
            //sceneType = SceneType.Huntingground;
        }

        public override void Init()
        {
            Map.LoadMap("Huntingground");

            MonsterSpawn();
        }

        public void MonsterSpawn()
        {
            for (int i = 0; i < 15; i++)
            {
                Monster monster = ObjectManager.Instance.Add<Monster>();
                monsters.Add(monster);
            }

            Random random = new Random();
            //몬스터 위치 랜덤 설정
            foreach (Monster monster in monsters)
            {
                while (true)
                {
                    Vector2Int pos = new Vector2Int()
                    {
                        x = random.Next(-20, 20),
                        y = random.Next(-20, 20)
                    };
                    if (Map.CanGo(pos))
                    {
                        monster.CellPos = pos;
                        EnterGame(monster);
                        break;
                    }
                }
            }
        }
    }
}
