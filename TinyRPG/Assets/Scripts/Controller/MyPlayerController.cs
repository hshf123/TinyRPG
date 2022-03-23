using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MyPlayerController : PlayerController
{
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

    protected override void UpdateIdle()
    {
        // 이동 상태로 갈지 확인
        if (Dir != MoveDir.None)
        {
            State = CreatureState.Moving;
            return;
        }

        // 스킬 상태로 갈지 확인
        if (Input.GetKey(KeyCode.A))
        {
            _coSkill = StartCoroutine("CoAutoAttack");
            State = CreatureState.Skill;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            _coSkill = StartCoroutine("CoArrowSkill");
            State = CreatureState.Skill;
        }
    }

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
    void Portal() // 해당 좌표의 포탈이 어디로 이어지는지 찾고 해당 씬을 로드
    {
        string mapName;
        Managers.Map.PortalPos.TryGetValue(CellPos, out mapName);
        Define.Scene sceneType = Managers.Scene.GetSceneType(mapName);
        if (sceneType != Define.Scene.Unknown)
        {
            Managers.Scene.LoadScene(sceneType);
        }
    }
}
