using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class SkillMonsterController : MonsterController
{
    Coroutine _coSkill;

    public override CreatureState State
    {
        get { return PosInfo.State; }
        set
        {
            if (PosInfo.State == value)
                return;

            base.State = value;
            if (_coSkill != null)
            {
                StopCoroutine(_coSkill);
                _coSkill = null;
            }
        }
    }

    [SerializeField]
    float _skillRange = 10.0f;

    protected override void Init()
    {
        base.Init();
        _searchRange = 10.0f;
    }

    protected override void MoveToNextPos()
    {
        Vector3Int destPos = _destPos;

        if (_target != null)
        {
            destPos = _target.GetComponent<CreatureController>().CellPos;

            Vector3Int dir = destPos - CellPos; // 위치 벡터 추출
            if (dir.magnitude <= _skillRange && (dir.x == 0 || dir.y == 0)) // 거리 추출
            {
                Dir = GetDirFromVec(dir);
                State = CreatureState.Skill;
                _coSkill = StartCoroutine("CoArrowSkill");
                return;
            }
        }

        List<Vector3Int> path = Managers.Map.FindPath(CellPos, destPos, ignoreDestCollision: true);
        if (path.Count < 2 || (_target != null && path.Count > 10))
        {
            _target = null;
            State = CreatureState.Idle;
            return;
        }

        Vector3Int nextPos = path[1];
        Vector3Int moveCellDir = nextPos - CellPos; // 목적지 방향 벡터

        // 방향 설정
        Dir = GetDirFromVec(moveCellDir);

        // 충돌, 피격 판정
        if (Managers.Map.CanGo(nextPos))
        {
            GameObject target = Managers.Object.FindCreature(nextPos);
            if (target != null)
            {
                if (target == _target)
                {
                    _target.GetComponent<CreatureController>().OnDamage();
                    State = CreatureState.Idle;
                }
                else
                {
                    State = CreatureState.Idle;
                }
            }
            else
            {
                CellPos = nextPos;
            }
        }
        else
        {
            State = CreatureState.Idle;
        }
    }

    IEnumerator CoAutoAttack() // 아직 사용할지는 미정
    {
        // 피격 판정
        GameObject go = Managers.Object.FindCreature(GetFrontCellPos());
        if (go != null)
        {
            CreatureController cc = go.GetComponent<CreatureController>();
            if (cc != null)
                cc.OnDamage();
        }

        // 대기 시간
        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Idle;
        _coSkill = null;
    }
    IEnumerator CoArrowSkill()
    {
        GameObject arrow = Managers.Resource.Instantiate("Misc/Arrow");
        ArrowController ac = arrow.GetComponent<ArrowController>();
        ac.Dir = Dir;
        ac.CellPos = CellPos;

        // 대기 시간
        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Idle;
        _coSkill = null;
    }
}
