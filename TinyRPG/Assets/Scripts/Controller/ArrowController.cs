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

        base.Init();
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
}
