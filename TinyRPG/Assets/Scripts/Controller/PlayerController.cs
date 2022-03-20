using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerController : CreatureController
{
    protected override void Init()
    {
        base.Init();
    }

    protected override void UpdateController()
    {
        GetDirInput();
        base.UpdateController();
        if (Managers.Map.IsPortal(CellPos))
            Portal();
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
    void Portal() // �ش� ��ǥ�� ��Ż�� ���� �̾������� ã�� �ش� ���� �ε�
    {
        string mapName;
        Managers.Map.PortalPos.TryGetValue(CellPos, out mapName);
        Define.Scene sceneType = Managers.Scene.GetSceneType(mapName);
        if(sceneType!=Define.Scene.Unknown)
        {
            Managers.Scene.LoadScene(sceneType);
        }
    }
}
