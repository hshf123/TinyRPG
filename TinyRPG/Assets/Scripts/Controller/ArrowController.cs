using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ArrowController : BaseController
{
    int movingCount = 0;

    protected override void Init()
    {
        switch(_lastDir)
        {
            case MoveDir.Up:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case MoveDir.Right:
                transform.rotation = Quaternion.Euler(0, 0, -90);
                break;
            case MoveDir.Down:
                transform.rotation = Quaternion.Euler(0, 0, -180);
                break;
            case MoveDir.Left:
                transform.rotation = Quaternion.Euler(0, 0, -270);
                break;
        }

        _animator = GetComponent<Animator>();
        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.8f, 0);
        transform.position = pos;
    }

    protected override void UpdateAnimation() // 굳이 실행해 줄 애니메이션이 없으므로 빈 깡통으로 둬서 우회
    {

    }

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
                GameObject go = Managers.Object.Find(destPos);
                if (Managers.Object.Find(destPos) == null)
                {
                    CellPos = destPos;
                    movingCount++;
                    if(movingCount == 8)
                        Managers.Resource.Destroy(gameObject);
                }
                else
                {
                    // 피격 판정
                    CreatureController cc = go.GetComponent<CreatureController>();
                    if (cc != null)
                        cc.OnDamage();

                    Managers.Resource.Destroy(gameObject); // 화살 제거
                }
            }
            else
            {
                // 화살은 자기 자신 소멸, +피격 이펙트 넣어줄까?
                Managers.Resource.Destroy(gameObject);
            }
        }
    }
    protected override void UpdateMoving() // 움직일 때 한칸 단위로 움직이게 해준다.
    {
        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.8f, 0); // 목적지 좌표 추출
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
}
