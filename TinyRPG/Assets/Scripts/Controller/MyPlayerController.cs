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
        // 카메라 관련
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    // IDLE상태에서 Moving으로 갈지, Skill로 갈지
    protected override void UpdateIdle()
    {
        // 이동 상태로 갈지 확인
        if (_keyPressed)
        {
            State = CreatureState.Moving;
            return;
        }

        // 스킬 상태로 갈지 확인
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
    void Portal() // 해당 좌표의 포탈이 어디로 이어지는지 찾고 해당 씬을 로드
    {
        string mapName;
        Managers.Map.PortalPos.TryGetValue(CellPos, out mapName);
        SceneType sceneType = Managers.Scene.GetSceneType(mapName);
        if (sceneType != SceneType.Unknown)
        {
            Managers.Scene.LoadScene(sceneType);
        }
    }
    protected override void CheckUpdatedFlag() // flag를 체크해서 상태변화(State, Position, Dir)가 일어나면 패킷을 전송
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
