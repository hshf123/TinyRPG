using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDick();
}

public class DataManager
{
    public Dictionary<int, Skill> SkillDict { get; private set; } = new Dictionary<int, Skill>();

    public void Init()
    {
        SkillDict = LoadJSon<SkillData, int, Skill>("SkillData").MakeDick();
    }

    Loader LoadJSon<Loader,Key,Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
        return JsonUtility.FromJson<Loader>(textAsset.text);
    }
}
