using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerController : MonoBehaviour
{
    public float _speed = 5.0f;

    Vector3Int _cellPos = Vector3Int.zero + new Vector3Int(1, 0, 0);
    bool _isMoving;
    Animator _animator;

    MoveDir _dir = MoveDir.Down;
    public MoveDir Dir
    {
        get { return _dir; }
        set
        {
            if (_dir != value)
            {
                switch (value)
                {
                    case MoveDir.Up:
                        _animator.Play("WALK_BACK");
                        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                        break;
                    case MoveDir.Right:
                        _animator.Play("WALK_RIGHT");
                        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                        break;
                    case MoveDir.Down:
                        _animator.Play("WALK_FRONT");
                        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                        break;
                    case MoveDir.Left:
                        _animator.Play("WALK_RIGHT");
                        transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                        break;
                    case MoveDir.None:
                        if (_dir == MoveDir.Up)
                        {
                            _animator.Play("IDLE_BACK");
                            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                        }
                        else if (_dir == MoveDir.Right)
                        {
                            _animator.Play("IDLE_RIGHT");
                            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                        }
                        else if (_dir == MoveDir.Left)
                        {
                            _animator.Play("IDLE_RIGHT");
                            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                        }
                        else
                        {
                            _animator.Play("IDLE_FRONT");
                            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                        }
                        break;
                }
                _dir = value;
            }
        }
    }

    void Start()
    {
        _animator = GetComponent<Animator>();
        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(_cellPos) + new Vector3(0.5f, 1.0f, 0);
        transform.position = pos;
    }

    void Update()
    {
        GetDirInput();
        NextPos();
        MoveToNextPos();
    }

    void LateUpdate()
    {
        // ī�޶� ����
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    // �÷��̾� �̵� ����
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
    void NextPos() // �̵������� ��Ȳ�̸� ��ǥ�� �̵��Ѵ�.
    {
        if (_isMoving == false && _dir != MoveDir.None)
        {
            Vector3Int destPos = _cellPos; // �������� �� ������
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
                _cellPos = destPos;
                _isMoving = true;
            }
        }
    }
    void MoveToNextPos() // �� ĭ ������ �����̰Բ� ����
    {
        if (_isMoving == false)
            return;

        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(_cellPos) + new Vector3(0.5f, 1.0f, 0); // ������ ��ǥ ����
        Vector3 moveDir = destPos - transform.position; // ���� ���� ����

        // ���� ���� üũ
        float dist = moveDir.magnitude; // ���� ������ ũ�� = ������ ������ �Ÿ�
        if (dist < _speed * Time.deltaTime) // �� ���� �̵��� �� �ִ� �Ÿ����� �۴ٰ� �ϸ� �����ߴٰ� ����.
        {
            transform.position = destPos;
            _isMoving = false;
        }
        else
        {
            transform.position += moveDir.normalized * _speed * Time.deltaTime; // ������ �������� ����
            _isMoving = true;
        }
    }

}
