using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BaseController : MonoBehaviour
{
    [SerializeField]
    protected float _speed = 10.0f;

    public Vector3Int CellPos { get; set; } = Vector3Int.zero + new Vector3Int(1, 0, 0);
    protected Animator _animator;
    protected SpriteRenderer _sprite;

    protected CreatureState _state = CreatureState.Idle;
    [SerializeField]
    public virtual CreatureState State
    {
        get { return _state; }
        set
        {
            if (_state == value)
                return;

            _state = value;
            // �ִϸ��̼� ó��
            UpdateAnimation();

        }
    }

    protected MoveDir _lastDir = MoveDir.Down; // ���������� �ٶ󺸰��ִ� ����
    protected MoveDir _dir = MoveDir.Down;
    public MoveDir Dir
    {
        get { return _dir; }
        set
        {
            if (_dir == value)
                return;

            _dir = value;
            if (value != MoveDir.None)
                _lastDir = value;

            UpdateAnimation();
        }
    }

    public Vector3Int GetFrontCellPos()
    {
        Vector3Int cellPos = CellPos;

        switch (_lastDir)
        {
            case MoveDir.Up:
                cellPos += Vector3Int.up;
                break;
            case MoveDir.Right:
                cellPos += Vector3Int.right;
                break;
            case MoveDir.Down:
                cellPos += Vector3Int.down;
                break;
            case MoveDir.Left:
                cellPos += Vector3Int.left;
                break;
        }

        return cellPos;
    }

    protected virtual void UpdateAnimation()
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

    protected virtual void Init()
    {
        _animator = GetComponent<Animator>();
        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 1.0f, 0);
        transform.position = pos;
    }

    protected virtual void UpdateController()
    {
        switch (State)
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

    // ��ü �̵� ����
    protected virtual void UpdateIdle()
    {
        
    }
    protected virtual void UpdateMoving() // ������ �� ��ĭ ������ �����̰� ���ش�.
    {
        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 1.0f, 0); // ������ ��ǥ ����
        Vector3 moveDir = destPos - transform.position; // ���� ���� ����

        // ���� ���� üũ
        float dist = moveDir.magnitude; // ���� ������ ũ�� = ������ ������ �Ÿ�
        if (dist < _speed * Time.deltaTime) // �� ���� �̵��� �� �ִ� �Ÿ����� �۴ٰ� �ϸ� �����ߴٰ� ����.
        {
            transform.position = destPos;
            // ����ĭ �̵�
            MoveToNextPos();
        }
        else
        {
            transform.position += moveDir.normalized * _speed * Time.deltaTime; // ������ �������� ����
            State = CreatureState.Moving;
        }
    }
    protected virtual void MoveToNextPos()
    {
        if(_dir == MoveDir.None)
        {
            State = CreatureState.Idle;
            return;
        }
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

        if (Managers.Map.CanGo(destPos))
        {
            if (Managers.Object.Find(destPos) == null)
            {
                CellPos = destPos;
            }
        }
    }
    protected virtual void UpdateSkill()
    {

    }
    protected virtual void UpdateDead()
    {

    }
}