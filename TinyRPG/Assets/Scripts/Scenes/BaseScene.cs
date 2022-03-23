using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseScene : MonoBehaviour
{
    public SceneType Type { get; protected set; } = SceneType.Unknown;

    void Awake()
    {
        Init();

        Screen.SetResolution(800, 600, false);
    }

    void Start()
    {
        
    }

    protected virtual void Init()
    {
        Object obj = GameObject.FindObjectOfType(typeof(EventSystem));
        if (obj == null)
            Managers.Resource.Instantiate("UI/EventSystem").name = "@EventSystem";
    }

    public abstract void Clear();
}
