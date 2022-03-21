using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerController : CreatureController
{
    Coroutine _coSkill;
    bool _aors;

    protected override void Init()
    {
        base.Init();
    }

    protected override void UpdateAnimation()
    {
        if (_state == CreatureState.Idle)
        {
            switch (_lastDir)
            {
                case MoveDir.Up:
                    _animator.Play("IDLE_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Right:
                    _animator.Play("IDLE_RIGHT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("IDLE_RIGHT");
                    _sprite.flipX = true;
                    break;
                case MoveDir.Down:
                    _animator.Play("IDLE_FRONT");
                    _sprite.flipX = false;
                    break;
            }
        }
        else if (_state == CreatureState.Moving)
        {
            switch (_dir)
            {
                case MoveDir.Up:
                    _animator.Play("WALK_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Right:
                    _animator.Play("WALK_RIGHT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("WALK_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("WALK_RIGHT");
                    _sprite.flipX = true;
                    break;
            }
        }
        else if (_state == CreatureState.Skill)
        {
            switch (_lastDir)
            {
                case MoveDir.Up:
                    _animator.Play(_aors ? "ATTACK_BACK" : "ATTACK_WEAPON_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Right:
                    _animator.Play(_aors ? "ATTACK_RIGHT" : "ATTACK_WEAPON_RIGHT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play(_aors ? "ATTACK_FRONT" : "ATTACK_WEAPON_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play(_aors ? "ATTACK_RIGHT" : "ATTACK_WEAPON_RIGHT");
                    _sprite.flipX = true;
                    break;
            }
        }
        else
        {
            // TODO
        }
    }

    protected override void UpdateController()
    {
        switch (State)
        {
            case CreatureState.Idle:
                GetDirInput();
                GetIdleInput();
                break;
            case CreatureState.Moving:
                GetDirInput();
                break;
        }

        base.UpdateController();
        if (Managers.Map.IsPortal(CellPos))
            Portal();
    }

    void LateUpdate()
    {
        // 카메라 관련
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    // 플레이어 이동 관련
    void GetDirInput()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            Dir = MoveDir.Up;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            Dir = MoveDir.Right;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            Dir = MoveDir.Down;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            Dir = MoveDir.Left;
        }
        else
        {
            Dir = MoveDir.None;
        }
    }
    void GetIdleInput()
    {
        if (Input.GetKey(KeyCode.A))
        {
            _coSkill = StartCoroutine("CoAutoAttack");
            State = CreatureState.Skill;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            _coSkill = StartCoroutine("CoArrowSkill");
            State = CreatureState.Skill;
        }
    }
    void Portal() // 해당 좌표의 포탈이 어디로 이어지는지 찾고 해당 씬을 로드
    {
        string mapName;
        Managers.Map.PortalPos.TryGetValue(CellPos, out mapName);
        Define.Scene sceneType = Managers.Scene.GetSceneType(mapName);
        if (sceneType != Define.Scene.Unknown)
        {
            Managers.Scene.LoadScene(sceneType);
        }
    }

    IEnumerator CoAutoAttack()
    {
        // 피격 판정
        GameObject go = Managers.Object.Find(GetFrontCellPos());
        if (go != null)
        {
            CreatureController cc = go.GetComponent<CreatureController>();
            if (cc != null)
                cc.OnDamage();
        }

        // 대기 시간
        _aors = true;
        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Idle;
        _coSkill = null;
    }

    IEnumerator CoArrowSkill()
    {
        GameObject arrow = Managers.Resource.Instantiate("Misc/Arrow");
        ArrowController ac = arrow.GetComponent<ArrowController>();
        ac.Dir = _lastDir;
        ac.CellPos = CellPos;

        // 대기 시간
        _aors = false;
        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Idle;
        _coSkill = null;
    }
}
