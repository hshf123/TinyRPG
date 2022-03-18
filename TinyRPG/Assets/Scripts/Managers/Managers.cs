using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers s_Instance;
    static Managers Instance { get { Init(); return s_Instance; } }

    DataManager _data = new DataManager();
    PoolManager _pool = new PoolManager();
    ResourceManager _resource = new ResourceManager();
    SceneManagerEx _scene = new SceneManagerEx();
    SoundManger _sound = new SoundManger();
    UI_Manager _ui = new UI_Manager();

    public static DataManager Data { get { return Instance._data; } }
    public static PoolManager Pool { get { return Instance._pool; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static SceneManagerEx Scene { get { return Instance._scene; } }
    public static SoundManger Sound { get { return Instance._sound; } }
    public static UI_Manager UI { get { return Instance._ui; } }

    void Start()
    {
    
    }

    void Update()
    {
    }

    static void Init()
    {
        if (s_Instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject() { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            s_Instance = go.GetComponent<Managers>();

            s_Instance._data.Init();
            s_Instance._pool.Init();
            s_Instance._sound.Init();
        }
    }

    public static void Clear()
    {
        Sound.Clear();
        Scene.Clear();
        UI.Clear();
        Pool.Clear();
    }
}
