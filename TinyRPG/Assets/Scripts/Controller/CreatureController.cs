using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CreatureController : BaseController
{
    protected override void UpdateAnimation()
    {
        if(_state == CreatureState.Idle)
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
        else if(_state == CreatureState.Moving)
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
        _sprite = GetComponent<SpriteRenderer>();
        Init();
    }

    void Update()
    {
        UpdateController();
    }

    protected override void Init()
    {
        _animator = GetComponent<Animator>();
        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 1.0f, 0);
        transform.position = pos;
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
    protected override void UpdateIdle() // 이동가능한 상황인지 체크하고, 갈 수 있는 곳이면 이동.
    {
        if (_dir != MoveDir.None)
        {
            Vector3Int destPos = CellPos; // 목적지가 될 포지션
            switch (Dir)
            {
                case MoveDir.Up:
                    destPos += Vector3Int.up;
                    break;
                case MoveDir.Right:
                    destPos += Vector3Int.right;
                    break;
                case MoveDir.Down:
                    destPos += Vector3Int.down;
                    break;
                case MoveDir.Left:
                    destPos += Vector3Int.left;
                    break;
            }

            State = CreatureState.Moving; 
            if (Managers.Map.CanGo(destPos))
            {
                if(Managers.Object.Find(destPos) == null)
                {
                    CellPos = destPos;
                }
            }
        }
    }
    protected override void UpdateMoving() // 움직일 때 한칸 단위로 움직이게 해준다.
    {
        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 1.0f, 0); // 목적지 좌표 추출
        Vector3 moveDir = destPos - transform.position; // 방향 벡터 추출

        // 도착 여부 체크
        float dist = moveDir.magnitude; // 방향 벡터의 크기 = 목적지 까지의 거리
        if (dist < _speed * Time.deltaTime) // 한 번에 이동할 수 있는 거리보다 작다고 하면 도착했다고 인정.
        {
            transform.position = destPos;
            // 예외적으로 애니메이션을 직접 컨트롤
            _state = CreatureState.Idle; // 여기까지만 하면 멈췄을 때 애니메이션이 안나옴
            if (_dir == MoveDir.None) // 진짜로 키보드를 뗀 상태라면
                UpdateAnimation();
        }
        else
        {
            transform.position += moveDir.normalized * _speed * Time.deltaTime; // 목적지 방향으로 전진
            State = CreatureState.Moving;
        }
    }
    protected override void UpdateSkill()
    {

    }
    protected override void UpdateDead()
    {

    }

    // 피격 판정
    public virtual void OnDamage()
    {
        GameObject effect = Managers.Resource.Instantiate("Effect/DeathEffect");
        effect.transform.position = gameObject.transform.position;
        effect.GetComponent<Animator>().Play("DEATH_EFFECT");
        GameObject.Destroy(effect, 0.5f);
        Managers.Object.Remove(gameObject);
        Managers.Resource.Destroy(gameObject);
    }
}
