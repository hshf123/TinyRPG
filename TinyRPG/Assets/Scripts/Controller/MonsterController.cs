using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{
    Coroutine _coPatrol;
    Coroutine _coSearchPlayer;
    [SerializeField]
    protected Vector3Int _destPos;

    [SerializeField]
    protected GameObject _target;

    [SerializeField]
    protected float _searchRange = 5.0f;

    public override CreatureState State
    {
        get { return PosInfo.State; }
        set
        {
            if (PosInfo.State == value)
                return;

            base.State = value;
            if (_coPatrol != null)
            {
                StopCoroutine(_coPatrol);
                _coPatrol = null;
            }
            if (_coSearchPlayer != null)
            {
                StopCoroutine(_coSearchPlayer);
                _coSearchPlayer = null;
            }
        }
    }

    protected override void Init()
    {
        base.Init();
        _speed = 5.0f;
        State = CreatureState.Idle;
        Dir = MoveDir.None;
    }

    protected override void UpdateIdle()
    {
        base.UpdateIdle();

        if (_coPatrol == null)
            _coPatrol = StartCoroutine("CoPatrol");
        if (_coSearchPlayer == null)
            _coSearchPlayer = StartCoroutine("CoSearchPlayer");
    }
    protected override void MoveToNextPos()
    {
        Vector3Int destPos = _destPos;

        if (_target != null)
        {
            destPos = _target.GetComponent<CreatureController>().CellPos;
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
            GameObject target = Managers.Object.Find(nextPos);
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

    public MoveDir GetDirFromVec(Vector3Int dir)
    {
        if (dir.x > 0)
            return MoveDir.Right;
        else if (dir.x < 0)
            return MoveDir.Left;
        else if (dir.y > 0)
            return MoveDir.Up;
        else if (dir.y < 0)
            return MoveDir.Down;
        else
            return MoveDir.None;
    }


    IEnumerator CoPatrol()
    {
        int waitSeconds = Random.Range(1, 4);
        yield return new WaitForSeconds(waitSeconds);

        for (int i = 0; i < 10; i++)
        {
            int x = Random.Range(-2, 3);
            int y = Random.Range(-2, 3);
            Vector3Int randPos = CellPos + new Vector3Int(x, y, 0);

            if (Managers.Map.CanGo(randPos) && Managers.Object.Find(randPos) == null)
            {
                _destPos = randPos;
                State = CreatureState.Moving;
                yield break;
            }
        }

        State = CreatureState.Idle;
    }
    IEnumerator CoSearchPlayer()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

            if (_target != null)
                continue;

            _target = Managers.Object.Find((go) =>
            {
                PlayerController pc = go.GetComponent<PlayerController>();
                if (pc == null)
                    return false;

                Vector3Int dir = pc.CellPos - CellPos;
                if (dir.magnitude < _searchRange)
                    return false;

                return true;
            });
        }
    }
}
