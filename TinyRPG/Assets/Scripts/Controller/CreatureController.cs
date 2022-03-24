using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CreatureController : BaseController
{
    protected override void UpdateAnimation()
    {
        if(State == CreatureState.Idle)
        {
            switch(_lastDir)
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
        else if(State == CreatureState.Moving)
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
            // TODO
            switch (_lastDir)
            {
                case MoveDir.Up:
                    _animator.Play("ATTACK_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Right:
                    _animator.Play("ATTACK_RIGHT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("ATTACK_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("ATTACK_RIGHT");
                    _sprite.flipX = true;
                    break;
            }
        }
        else
        {
            // TODO
        }
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        UpdateController();
    }

    protected override void Init()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 1.0f, 0);
        transform.position = pos;

        State = CreatureState.Idle;
        Dir = MoveDir.None;
        UpdateAnimation();
    }

    protected override void UpdateController()
    {
        switch(State)
        {
            case CreatureState.Idle:
                UpdateIdle();
                break;
            case CreatureState.Moving:
                UpdateMoving();
                break;
            case CreatureState.Skill:
                break;
            case CreatureState.Dead:
                break;
        }
    }

    // 크리쳐 이동 관련
    protected override void UpdateIdle()
    {
        base.UpdateIdle();
    }
    protected override void UpdateMoving() // 움직일 때 한칸 단위로 움직이게 해준다.
    {
        base.UpdateMoving();
    }
    protected override void UpdateSkill()
    {
        base.UpdateSkill();
    }
    protected override void UpdateDead()
    {
        base.UpdateDead();
    }

    // 패킷을 처음 받고 생성되었을 때 포지면 맞춰주기.
    public void SyncPos()
    {
        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 1.0f);
        transform.position = pos;
    }

    // 피격 판정
    public virtual void OnDamage() 
    {
        GameObject effect = Managers.Resource.Instantiate("Effect/DeathEffect");
        effect.transform.position = gameObject.transform.position;
        effect.GetComponent<Animator>().Play("DEATH_EFFECT");
        GameObject.Destroy(effect, 0.5f);

        Managers.Object.Remove(Id);
        Managers.Resource.Destroy(gameObject);
    }
}
