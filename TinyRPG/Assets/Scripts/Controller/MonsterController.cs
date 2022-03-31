using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{
    protected override void Init()
    {
        base.Init();
        State = CreatureState.Idle;
        Dir = MoveDir.Down;
    }

    protected override void UpdateIdle()
    {
        base.UpdateIdle();
    }

    public override void UseSkill(int skillId)
    {
    }
}
