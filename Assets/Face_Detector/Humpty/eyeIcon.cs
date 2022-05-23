using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eyeIcon : MonoBehaviour
{
    public OpenCvSharp.Demo.IfBlinked blinkDealer;

    public int endingNum = 0;

    public LayerMask mask;
    public GameObject playerHead;
    Camera cam;
    RaycastHit hit;
    private void Start()
    {

        cam = playerHead.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        //print(onObj);
        transform.LookAt(playerHead.transform.position);

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {
            if (hit.transform.name == transform.name)
            {
                blinkDealer.lookingAtColl = true;
                
                if (endingNum == 0)
                    blinkDealer.lookingAtColl = true;
                else if (endingNum == 1)
                    blinkDealer.lookingAtEnding1 = true;
                else if (endingNum == 2)
                    blinkDealer.lookingAtEnding2 = true;

            }
            else
            {
                blinkDealer.lookingAtColl = false;

                if (endingNum == 0)
                    blinkDealer.lookingAtColl = false;
                else if (endingNum == 1)
                    blinkDealer.lookingAtEnding1 = false;
                else if (endingNum == 2)
                    blinkDealer.lookingAtEnding2 = false;
            }
        }

    }
}
