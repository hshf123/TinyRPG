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

    protected override void UpdateAnimation() // ���� ������ �� �ִϸ��̼��� �����Ƿ� �� �������� �ּ� ��ȸ
    {

    }

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
                    // �ǰ� ����
                    CreatureController cc = go.GetComponent<CreatureController>();
                    if (cc != null)
                        cc.OnDamage();

                    Managers.Resource.Destroy(gameObject); // ȭ�� ����
                }
            }
            else
            {
                // ȭ���� �ڱ� �ڽ� �Ҹ�, +�ǰ� ����Ʈ �־��ٱ�?
                Managers.Resource.Destroy(gameObject);
            }
        }
    }
}
