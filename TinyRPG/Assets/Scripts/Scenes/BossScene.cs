using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        Type = SceneType.Boss;
        Managers.Map.LoadMap("Boss");
    }

    public override void Clear()
    {
    }
}
