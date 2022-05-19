using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTest : MonoBehaviour
{

    [SerializeField, Range(0f, 1f)]
    float scale;


    float oldDelta;

    private void Awake()
    {
        oldDelta = Time.fixedDeltaTime;
        //scale = Time.timeScale;
    }

    // Update is called once per frame
    void Update()
    {
        //Time.timeScale = scale;
    }
}
