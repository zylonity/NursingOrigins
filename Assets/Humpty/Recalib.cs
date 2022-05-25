using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Recalib : MonoBehaviour
{
    public GameObject webcam;
    public GameObject events;

    bool paused = false;
    bool cPressed = false;

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.C) && cPressed == false)
        {
            AudioListener.pause = true;
            webcam.SetActive(false);
            events.SetActive(false);
            Time.timeScale = 0;
            SceneManager.LoadScene(1, LoadSceneMode.Additive);
            cPressed = true;
        }

        if (SceneManager.GetSceneByBuildIndex(1).isLoaded)
        {
            paused = true;
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(1));
            Cursor.lockState = CursorLockMode.None;
        }

        if (!SceneManager.GetSceneByBuildIndex(1).isLoaded && paused)
        {
            paused = false;
            Time.timeScale = 1;
            AudioListener.pause = false;
            cPressed = false;
            webcam.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
        }


    }

    private void FixedUpdate()
    {
        
    }



}
