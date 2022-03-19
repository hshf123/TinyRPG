using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerController : MonoBehaviour
{
    public Grid _grid;
    public float _speed = 5.0f;

    Vector3Int _cellPos = Vector3Int.zero + new Vector3Int(1, 0, 0);
    MoveDir _dir = MoveDir.None;
    bool _isMoving;

    void Start()
    {
        Vector3 pos = _grid.CellToWorld(_cellPos) + new Vector3(0.5f, 0, 0);
        transform.position = pos;
    }

    void Update()
    {
        GetDirInput();
        NextPos(); 
        MoveToNextPos();
    }

    // 플레이어 이동 관련
    void GetDirInput()
    {
        if(Input.GetKey(KeyCode.UpArrow))
        {
            _dir = MoveDir.Up;
        }
        else if(Input.GetKey(KeyCode.RightArrow))
        {
            _dir = MoveDir.Right;
        }
        else if(Input.GetKey(KeyCode.DownArrow))
        {
            _dir = MoveDir.Down;
        }
        else if(Input.GetKey(KeyCode.LeftArrow))
        {
            _dir = MoveDir.Left;
        }
        else
        {
            _dir = MoveDir.None;
        }
    }
    void NextPos()
    {
        if(_isMoving==false)
        {
            switch (_dir)
            {
                case MoveDir.Up:
                    _cellPos += Vector3Int.up;
                    _isMoving = true;
                    break;
                case MoveDir.Right:
                    _cellPos += Vector3Int.right;
                    _isMoving = true;
                    break;
                case MoveDir.Down:
                    _cellPos += Vector3Int.down;
                    _isMoving = true;
                    break;
                case MoveDir.Left:
                    _cellPos += Vector3Int.left;
                    _isMoving = true;
                    break;
            }
        }
    }
    void MoveToNextPos()
    {
        if (_isMoving == false)
            return;

        Vector3 destPos = _grid.CellToWorld(_cellPos) + new Vector3(0.5f, 0, 0); // 목적지 좌표 추출
        Vector3 moveDir = destPos - transform.position; // 방향 벡터 추출

        // 도착 여부 체크
        float dist = moveDir.magnitude; // 방향 벡터의 크기 = 목적지 까지의 거리
        if(dist < _speed * Time.deltaTime) // 한 번에 이동할 수 있는 거리보다 작다고 하면 도착했다고 인정.
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
