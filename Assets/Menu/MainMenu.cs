using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public GameObject mainMenu;
    public GameObject settingsMenu;
    public GameObject shortSettings;
    public GameObject creditsMenu;
    public GameObject CamOrManual;
    public GameObject warningMenu;
    public GameObject accesibilityMenu;
    

    public void PlayGame()
    {
        //PlayerPrefs.DeleteAll();
        mainMenu.SetActive(false);
        shortSettings.SetActive(true);
    }
    
    public void Settings()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }
    
    public void Exit()
    {
        Application.Quit();
    }

    public void Credits()
    {
        mainMenu.SetActive(false);
    }

    public void Continue()
    {
        SceneManager.LoadScene(1);
    }
    
    public void WarningBack()
    {
        warningMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void QuestionsBack()
    {
        CamOrManual.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void Webcam()
    {
        CamOrManual.SetActive(false);
        warningMenu.SetActive(true);
    }

    public void Manual()
    {
        PlayerPrefs.SetString("Eye", "Manual");
        SceneManager.LoadScene(2);
    }




}
