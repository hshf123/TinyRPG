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

    // ũ���� �̵� ����
    protected override void UpdateIdle() // �̵������� ��Ȳ���� üũ�ϰ�, �� �� �ִ� ���̸� �̵�.
    {
        if (_dir != MoveDir.None)
        {
            Vector3Int destPos = CellPos; // �������� �� ������
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
    protected override void UpdateMoving() // ������ �� ��ĭ ������ �����̰� ���ش�.
    {
        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 1.0f, 0); // ������ ��ǥ ����
        Vector3 moveDir = destPos - transform.position; // ���� ���� ����

        // ���� ���� üũ
        float dist = moveDir.magnitude; // ���� ������ ũ�� = ������ ������ �Ÿ�
        if (dist < _speed * Time.deltaTime) // �� ���� �̵��� �� �ִ� �Ÿ����� �۴ٰ� �ϸ� �����ߴٰ� ����.
        {
            transform.position = destPos;
            // ���������� �ִϸ��̼��� ���� ��Ʈ��
            _state = CreatureState.Idle; // ��������� �ϸ� ������ �� �ִϸ��̼��� �ȳ���
            if (_dir == MoveDir.None) // ��¥�� Ű���带 �� ���¶��
                UpdateAnimation();
        }
        else
        {
            transform.position += moveDir.normalized * _speed * Time.deltaTime; // ������ �������� ����
            State = CreatureState.Moving;
        }
    }
    protected override void UpdateSkill()
    {

    }
    protected override void UpdateDead()
    {

    }

    // �ǰ� ����
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
