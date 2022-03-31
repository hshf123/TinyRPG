using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerController : CreatureController
{
    protected Coroutine _coSkill;
    bool _aors; // Auto or Skill(Arrow)

    protected override void Init()
    {
        base.Init();
    }

    protected override void UpdateAnimation()
    {
        if (_animator == null || _sprite == null)
            return;

        if (State == CreatureState.Idle)
        {
            switch (Dir)
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
        else if (State == CreatureState.Moving)
        {
            switch (Dir)
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
        else if (State == CreatureState.Skill)
        {
            switch (Dir)
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
        base.UpdateController();
    }

    protected virtual void CheckUpdatedFlag()
    {
        
    }

    public override void UseSkill(int skillId)
    {
        if(skillId == 1)
        {
            _coSkill = StartCoroutine("CoAutoAttack");
        }
        else if(skillId == 2)
        {
            _coSkill = StartCoroutine("CoArrowSkill");
        }
    }

    IEnumerator CoAutoAttack()
    {
        // 대기 시간
        _aors = true;
        State = CreatureState.Skill;
        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Idle;
        _coSkill = null;
        CheckUpdatedFlag(); 
    }

    IEnumerator CoArrowSkill()
    {
        // 대기 시간
        _aors = false;
        State = CreatureState.Skill;
        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Idle;
        _coSkill = null;
        CheckUpdatedFlag();
    }
}
