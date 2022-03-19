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
    }

    public override void Clear()
    {

    }
}
