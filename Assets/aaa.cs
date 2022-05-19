using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class aaa : MonoBehaviour
{
    RawImage rawimage;
    void Start()
    {
        rawimage = GetComponent<RawImage>();
        WebCamTexture webcamTexture = new WebCamTexture(PlayerPrefs.GetString("Camera"));
        rawimage.texture = webcamTexture;
        rawimage.material.mainTexture = webcamTexture;
        webcamTexture.Play();
    }

}
