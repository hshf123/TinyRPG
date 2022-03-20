using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Test : MonoBehaviour
{
    void Start()
    {
        Transform go = GetComponent<Transform>();
        Transform[] cs = go.GetComponentsInChildren<Transform>();
        foreach (Transform c in cs)
        {
            Debug.Log(c.name);
        }
    }

    void Update()
    {

    }
}
