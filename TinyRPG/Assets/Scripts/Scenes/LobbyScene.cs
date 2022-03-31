using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        Type = SceneType.Lobby;
        Managers.Map.LoadMap("Lobby");
    }

    private void Update()
    {
        
    }

    public override void Clear()
    {
        
    }
}
