using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntinggroundScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        Type = SceneType.Huntingground;
        Managers.Map.LoadMap("Huntingground");

        //GameObject player = Managers.Resource.Instantiate("Creature/Player");
        //player.name = "Player";
        //Managers.Object.Add(player);
        
    }

    public override void Clear()
    {

    }
}
