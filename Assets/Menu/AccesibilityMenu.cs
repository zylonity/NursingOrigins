using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccesibilityMenu : MonoBehaviour
{

    public Button left;
    public Button right;
    public Button both;

    public Button subLeft;
    public Button subRight;

    public void LeftEye()
    {
        PlayerPrefs.SetString("Eye", "Left");
        left.interactable = false;
        right.interactable = true;
        both.interactable = true;
    }
    
    public void RightEye()
    {
        PlayerPrefs.SetString("Eye", "Right");
        left.interactable = true;
        right.interactable = false;
        both.interactable = true;

    }

    public void BothEyes()
    {
        PlayerPrefs.SetString("Eye", "Both");
        left.interactable = true;
        right.interactable = true;
        both.interactable = false;

    }
    
    public void SubsOn()
    {
        PlayerPrefs.SetString("Subtitles", "On");
        subLeft.interactable = false;
        subRight.interactable = true;
    }

    public void SubsOff()
    {
        PlayerPrefs.SetString("Subtitles", "Off");
        subLeft.interactable = true;
        subRight.interactable = false;
    }


}
