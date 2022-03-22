using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MultiPlayBuildAndRun
{
    [MenuItem("Tools/MultiPlay : 2")]
    static void MultiPlay_2()
    {
        PerformWin64Build(2);
    }
    [MenuItem("Tools/MultiPlay : 3")]
    static void MultiPlay_3()
    {
        PerformWin64Build(3);
    }
    [MenuItem("Tools/MultiPlay : 4")]
    static void MultiPlay_4()
    {
        PerformWin64Build(4);
    }

    static void PerformWin64Build(int playerCount)
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(
            BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);

        for(int i=1; i<=playerCount; i++)
        {
            BuildPipeline.BuildPlayer(GetScenePaths(),
                "../Build/Win64/" + GetProjectName() + i.ToString() + "/" + GetProjectName() + i.ToString() + ".exe",
                BuildTarget.StandaloneWindows64, BuildOptions.AutoRunPlayer);
        }
    }

    // 프로젝트 이름 불러오기
    static string GetProjectName()
    {
        string[] s = Application.dataPath.Split('/');
        return s[s.Length - 2];
    }

    // 빌드 할 때 넣어준 씬들 긁어오기
    static string[] GetScenePaths()
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];

        for(int i=0; i<scenes.Length;i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }

        return scenes;
    }
}
