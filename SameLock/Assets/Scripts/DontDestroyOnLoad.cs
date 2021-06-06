using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    public int obj;

    private void Awake()
    {
        GameObject[] objs;
        if (obj == 0)
            objs = GameObject.FindGameObjectsWithTag("NGapi");
        else objs = GameObject.FindGameObjectsWithTag("apiHandler");
        if (objs.Length > 1)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
}
