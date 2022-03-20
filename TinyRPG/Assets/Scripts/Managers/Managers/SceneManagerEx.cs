using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx
{
    public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }

    public void LoadScene(Define.Scene type)
    {
        Managers.Clear();
        SceneManager.LoadScene(GetSceneName(type));
    }

    public string GetSceneName(Define.Scene type)
    {
        return System.Enum.GetName(typeof(Define.Scene), type);
    }

    public Define.Scene GetSceneType(string name)
    {
        string[] names = System.Enum.GetNames(typeof(Define.Scene));
        for(int i=0; i<names.Length; i++)
        {
            if (name == names[i])
                return (Define.Scene)i;
        }

        return Define.Scene.Unknown;
    }

    public void Clear()
    {
        CurrentScene.Clear();
    }
}
