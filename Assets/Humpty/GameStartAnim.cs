using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartAnim : MonoBehaviour
{
    public GameObject nursery;
    public OpenCvSharp.Demo.IfBlinked bDect;

    public Animator blink;

    bool firstRun = false;
    
    public GameObject mainCam;
    public GameObject pusher;
    public AudioClip song;

    public GameObject sub1, sub2, sub3, sub4;

    public Rigidbody wall1, wall2, wall3, wall4;

    public AudioSource camMic;

    public bool breakWalls = false;

    private bool subTitles;


    private void Start()
    {
        if (PlayerPrefs.GetString("Subtitles") == "On")
            subTitles = true;
        else if (PlayerPrefs.GetString("Subtitles") == "Off")
            subTitles = false;

        camMic.clip = song;
        camMic.Play();
    }

    void Update()
    {

        if (subTitles && camMic.isPlaying)
        {
            if (camMic.time < 2.6f)
            {
                sub1.SetActive(true);
            }
            else if (camMic.time < 5.6f)
            {
                sub1.SetActive(false);
                sub2.SetActive(true);
            }
            else if (camMic.time < 8.7f)
            {
                sub2.SetActive(false);
                sub3.SetActive(true);
            }
            else if (camMic.time < 13.8f)
            {
                sub3.SetActive(false);
                sub4.SetActive(true);
            }
            else
            {
                sub4.SetActive(false);
            }


        }


        if (!camMic.isPlaying)
        {
            wall1.isKinematic = false;
            wall2.isKinematic = false;
            wall3.isKinematic = false;
            wall4.isKinematic = false;
            pusher.transform.localScale += new Vector3(0.005f, 0.005f, 0f);
            pusher.GetComponent<MeshRenderer>().enabled = false;
            if (!firstRun)
            {
                StartCoroutine(done());
                firstRun = true;
            }

        }

    }

    IEnumerator done()
    {
        yield return new WaitForSeconds(3);
        blink.SetTrigger("Blink");

        yield return new WaitForSeconds(1f);
        mainCam.SetActive(true);
        camMic.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.41f);
        blink.SetTrigger("Return");
        nursery.SetActive(false);

        bDect.gameStart = true;
        yield return null;
    }

}
