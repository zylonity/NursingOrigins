using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    public GameObject settingsMenu;
    public GameObject mainMenu;
    public GameObject warningMenu;
    public GameObject credits;
    public GameObject accesibility;


    public void Back()
    {
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void calibScene()
    {
        settingsMenu.SetActive(false);
        warningMenu.SetActive(true);
    }

    public void accesibilityMenu()
    {
        settingsMenu.SetActive(false);
        accesibility.SetActive(true);
    }

}
