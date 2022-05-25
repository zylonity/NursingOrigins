using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eyeIcon : MonoBehaviour
{
    public OpenCvSharp.Demo.IfBlinked blinkDealer;


    LayerMask blinkingIcons;
    public GameObject playerHead;
    Camera cam;
    RaycastHit hit;

    public string nameCol = null;

    private void Start()
    {

        cam = playerHead.GetComponent<Camera>();

    }

    


    void Update()
    {
        //print(onObj);
        transform.LookAt(playerHead.transform.position);

        blinkingIcons = LayerMask.GetMask("BlinkingIcon");

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, blinkingIcons))
        {
            blinkDealer.lookingAtColl = true;
            blinkDealer.lookingAt = hit.transform.name;
        }
        else
        {
            blinkDealer.lookingAtColl = false;
            nameCol = null;
            blinkDealer.lookingAt = null;
        }

    }
}
