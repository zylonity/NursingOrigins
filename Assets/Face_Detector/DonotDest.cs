using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonotDest : MonoBehaviour
{
    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("DontDestroy");


        DontDestroyOnLoad(this.gameObject);

    }
}
