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

    //protected override void MoveToNextPos()
    //{
    //    Vector3Int destPos = _destPos;

    //    if (_target != null)
    //    {
    //        destPos = _target.GetComponent<CreatureController>().CellPos;

    //        Vector3Int dir = destPos - CellPos; // ��ġ ���� ����
    //        if (dir.magnitude <= _skillRange && (dir.x == 0 || dir.y == 0)) // �Ÿ� ����
    //        {
    //            Dir = GetDirFromVec(dir);
    //            State = CreatureState.Skill;
    //            _coSkill = StartCoroutine("CoArrowSkill");
    //            return;
    //        }
    //    }

    //    List<Vector3Int> path = Managers.Map.FindPath(CellPos, destPos, ignoreDestCollision: true);
    //    if (path.Count < 2 || (_target != null && path.Count > 10))
    //    {
    //        _target = null;
    //        State = CreatureState.Idle;
    //        return;
    //    }

    //    Vector3Int nextPos = path[1];
    //    Vector3Int moveCellDir = nextPos - CellPos; // ������ ���� ����

    //    // ���� ����
    //    Dir = GetDirFromVec(moveCellDir);

    //    // �浹, �ǰ� ����
    //    if (Managers.Map.CanGo(nextPos))
    //    {
    //        GameObject target = Managers.Object.Find(nextPos);
    //        if (target != null)
    //        {
    //            if (target == _target)
    //            {
    //                _target.GetComponent<CreatureController>().OnDamage();
    //                State = CreatureState.Idle;
    //            }
    //            else
    //            {
    //                State = CreatureState.Idle;
    //            }
    //        }
    //        else
    //        {
    //            CellPos = nextPos;
    //        }
    //    }
    //    else
    //    {
    //        State = CreatureState.Idle;
    //    }
    //}
}
