using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : CreatureController
{
    protected override void Init()
    {
        base.Init();

        State = CreatureState.Idle;
        Dir = MoveDir.Down;
    }

    public override void UseSkill(int skillId)
    {

    }
}
