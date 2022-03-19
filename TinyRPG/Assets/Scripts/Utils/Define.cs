using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum MoveDir
    {
        None,
        Up,
        Right,
        Down,
        Left,
    }

    public enum Scene
    {
        Unknown,
        Login,
        Lobby,
        HuntingGround,
    }

    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount,
    }

    public enum UIEvent
    {
        Click,
        Drag,
    }

    public enum MouseEvent
    {
        Press,
        Click,
    }

    public enum CameraMode
    {
        QuarterView,
    }
}
