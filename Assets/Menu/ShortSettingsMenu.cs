using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ShortSettingsMenu : MonoBehaviour
{

    public Slider mouseSens;
    public GameObject shortSettings;
    public GameObject accesibilityMenu;
    public GameObject mainMenu;
    public GameObject camOrManual;
    

    public void Accessibility()
    {
        shortSettings.SetActive(false);
        accesibilityMenu.SetActive(true);
    }

    public void back()
    {
        accesibilityMenu.SetActive(false);
        shortSettings.SetActive(false);
        mainMenu.SetActive(true);

    }

    public void Continue()
    {
        shortSettings.SetActive(false);
        camOrManual.SetActive(true);
        PlayerPrefs.SetFloat("MouseSens", mouseSens.value);

        if (!PlayerPrefs.HasKey("Eye"))
        {
            PlayerPrefs.SetString("Eye", "Both");
        }

    }
    
    public void ChangeMouse()
    {
        PlayerPrefs.SetFloat("MouseSens", mouseSens.value);
    }

}
