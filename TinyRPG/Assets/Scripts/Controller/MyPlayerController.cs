using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Define;

public class MyPlayerController : PlayerController
{
    bool _keyPressed = false;

    protected override void Init()
    {
        base.Init();
    }

    protected override void UpdateController()
    {
        switch (State)
        {
            case CreatureState.Idle:
                GetDirInput();
                break;
            case CreatureState.Moving:
                GetDirInput();
                break;
        }

        base.UpdateController();
        if (Managers.Map.IsPortal(CellPos))
            Portal();
    }

    void LateUpdate()
    {
        // ī�޶� ����
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    // IDLE���¿��� Moving���� ����, Skill�� ����
    protected override void UpdateIdle()
    {
        // �̵� ���·� ���� Ȯ��
        if (_keyPressed)
        {
            State = CreatureState.Moving;
            return;
        }

        // ��ų ���·� ���� Ȯ��
        if (_coSkillCooltime == null && Input.GetKey(KeyCode.A))
        {
            Debug.Log("AutoAttack");

            C_Skill skillPacket = new C_Skill() { Info = new SkillInfo() };
            skillPacket.Info.SkillId = 1;
            Managers.Network.Send(skillPacket);

            _coSkillCooltime = StartCoroutine("CoSkillCooltime", 0.3f);
        }
        else if (_coSkillCooltime == null && Input.GetKey(KeyCode.S))
        {
            _coSkill = StartCoroutine("CoArrowSkill");
            State = CreatureState.Skill;
        }
    }

    Coroutine _coSkillCooltime;
    IEnumerator CoSkillCooltime(float time)
    {
        yield return new WaitForSeconds(time);
        _coSkillCooltime = null;
    }

    protected override void MoveToNextPos()
    {
        if (_keyPressed == false)
        {
            State = CreatureState.Idle;
            CheckUpdatedFlag();
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

        CheckUpdatedFlag();
    }

    void GetDirInput()
    {
        _keyPressed = true;

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
            _keyPressed = false;
        }
    }
    void Portal() // �ش� ��ǥ�� ��Ż�� ���� �̾������� ã�� �ش� ���� �ε�
    {
        string mapName;
        Managers.Map.PortalPos.TryGetValue(CellPos, out mapName);
        SceneType sceneType = Managers.Scene.GetSceneType(mapName);
        if (sceneType != SceneType.Unknown)
        {
            Managers.Scene.LoadScene(sceneType);
        }
    }
    protected override void CheckUpdatedFlag() // flag�� üũ�ؼ� ���º�ȭ(State, Position, Dir)�� �Ͼ�� ��Ŷ�� ����
    {
        if(_updated)
        {
            C_Move movePacket = new C_Move();
            movePacket.PosInfo = PosInfo;
            Managers.Network.Send(movePacket);
            _updated = false;
        }
    }
}
