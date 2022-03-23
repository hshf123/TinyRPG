using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx
{
    public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }

    public void LoadScene(SceneType type)
    {
        Managers.Clear();
        SceneManager.LoadScene(GetSceneName(type));
    }

    public string GetSceneName(SceneType type)
    {
        return System.Enum.GetName(typeof(SceneType), type);
    }

    public SceneType GetSceneType(string name)
    {
        string[] names = System.Enum.GetNames(typeof(SceneType));
        for(int i=0; i<names.Length; i++)
        {
            if (name == names[i])
                return (SceneType)i;
        }

        return SceneType.Unknown;
    }

    public void Clear()
    {
        CurrentScene.Clear();
    }
}
