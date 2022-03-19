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
    void NextPos() // 이동가능한 상황이면 좌표를 이동한다.
    {
        if (_isMoving == false && _dir != MoveDir.None)
        {
            Vector3Int destPos = _cellPos; // 목적지가 될 포지션
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
    void MoveToNextPos() // 한 칸 단위로 움직이게끔 해줌
    {
        if (_isMoving == false)
            return;

        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(_cellPos) + new Vector3(0.5f, 1.0f, 0); // 목적지 좌표 추출
        Vector3 moveDir = destPos - transform.position; // 방향 벡터 추출

        // 도착 여부 체크
        float dist = moveDir.magnitude; // 방향 벡터의 크기 = 목적지 까지의 거리
        if (dist < _speed * Time.deltaTime) // 한 번에 이동할 수 있는 거리보다 작다고 하면 도착했다고 인정.
        {
            transform.position = destPos;
            _isMoving = false;
        }
        else
        {
            transform.position += moveDir.normalized * _speed * Time.deltaTime; // 목적지 방향으로 전진
            _isMoving = true;
        }
    }

}
