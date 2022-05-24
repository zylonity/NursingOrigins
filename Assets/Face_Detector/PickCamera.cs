using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PickCamera : MonoBehaviour
{
    TMP_Dropdown webcamDropdown;
    List<string> deviceNames = new List<string>();
    
    void Start()
    {

        webcamDropdown = gameObject.GetComponent<TMP_Dropdown>();

        WebCamDevice[] devices = WebCamTexture.devices;


        foreach(WebCamDevice t in devices)
        {
            deviceNames.Add(t.name);
        }

        webcamDropdown.ClearOptions();
        webcamDropdown.AddOptions(deviceNames);

    }

    public void valueChanged()
    {
        PlayerPrefs.SetString("Camera", deviceNames[webcamDropdown.value]);
    }
}
