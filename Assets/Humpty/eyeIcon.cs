using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eyeIcon : MonoBehaviour
{
    public OpenCvSharp.Demo.IfBlinked blinkDealer;

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
            }
            else
            {
                blinkDealer.lookingAtColl = false;
            }
        }

    }
}
