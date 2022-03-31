using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class SkillMonsterController : MonsterController
{
    Coroutine _coSkill;

    protected override void Init()
    {
        base.Init();
    }

    public override void UseSkill(int skillId)
    {
        if (skillId == 1)
        {
            State = CreatureState.Skill;
        }
        else if (skillId == 2)
        {
            State = CreatureState.Skill;
        }
    }
}
