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
    public Slider mouseSens;

    private void Update()
    {
        PlayerPrefs.SetFloat("MouseSens", mouseSens.value);
    }


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
}
