using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveArmy : MonoBehaviour
{

    public float speedToMoveX = 2f;
    public float speedToMoveY = 0f;

    void Update()
    {
        transform.Translate(-((Vector3.forward * Time.deltaTime) * speedToMoveX));
        transform.Translate((Vector3.up * Time.deltaTime) * speedToMoveY);
    }
}
