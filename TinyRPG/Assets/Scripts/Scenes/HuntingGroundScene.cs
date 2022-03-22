using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntingGroundScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.HuntingGround;
        Managers.Map.LoadMap("HuntingGround");

        //GameObject player = Managers.Resource.Instantiate("Creature/Player");
        //player.name = "Player";
        //Managers.Object.Add(player);
        //// 몬스터 스폰
        //for (int i = 0; i < 4; i++)
        //{
        //    GameObject mole = Managers.Resource.Instantiate("Creature/Mole");
        //    GameObject redHero = Managers.Resource.Instantiate("Creature/RedHero");
        //    GameObject treant = Managers.Resource.Instantiate("Creature/Treant");
        //    mole.name = $"Mole_{i}";
        //    redHero.name = $"RedHero_{i}";
        //    treant.name = $"Treant_{i}";
        //    GameObject[] monsters = { mole, redHero, treant };
        //    // 몬스터 위치 랜덤 설정
        //    foreach (GameObject go in monsters)
        //    {
        //        while (true)
        //        {
        //            Vector3Int pos = new Vector3Int()
        //            {
        //                x = Random.Range(-20, 20),
        //                y = Random.Range(-10, 10),
        //            };
        //            if (Managers.Map.CanGo(pos))
        //            {
        //                MonsterController mc = go.GetComponent<MonsterController>();
        //                mc.CellPos = pos;
        //                Managers.Object.Add(go);
        //                break;
        //            }
        //        }
        //    }
        //}

    }

    public override void Clear()
    {

    }
}
