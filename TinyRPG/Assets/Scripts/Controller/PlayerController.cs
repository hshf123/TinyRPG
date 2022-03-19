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

    // �÷��̾� �̵� ����
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

        Vector3 destPos = _grid.CellToWorld(_cellPos) + new Vector3(0.5f, 0, 0); // ������ ��ǥ ����
        Vector3 moveDir = destPos - transform.position; // ���� ���� ����

        // ���� ���� üũ
        float dist = moveDir.magnitude; // ���� ������ ũ�� = ������ ������ �Ÿ�
        if(dist < _speed * Time.deltaTime) // �� ���� �̵��� �� �ִ� �Ÿ����� �۴ٰ� �ϸ� �����ߴٰ� ����.
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
