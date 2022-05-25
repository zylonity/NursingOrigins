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

    public Rigidbody wall1, wall2, wall3, wall4;

    public AudioSource camMic;

    public bool breakWalls = false;

    private void Start()
    {
        camMic.clip = song;
        camMic.Play();
    }

    void Update()
    {
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
