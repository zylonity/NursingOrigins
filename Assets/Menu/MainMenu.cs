using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public GameObject mainMenu;
    public GameObject settingsMenu;
    public GameObject creditsMenu;
    public GameObject warningMenu;

    public void PlayGame()
    {
        mainMenu.SetActive(false);
        warningMenu.SetActive(true);
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
    
}
