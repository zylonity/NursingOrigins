namespace OpenCvSharp.Demo
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.SceneManagement;
    using OpenCvSharp;
    using System.IO;
    using System.Linq;

    public class BlinkDect : MonoBehaviour
    {

        public delegate void Blinked();
        public static event Blinked onBlink;


        public bool manualBlink = false;

        public GameObject camHandle;

        public FaceDetectorScene faceDect;
        bool blinked = false;
    
        public float currentLeftArea, currentRightArea;

        float finalLeft, finalRight;
        float blinkDura;




        void Start()
        {
            finalLeft = PlayerPrefs.GetFloat("FinalLeft");
            finalRight = PlayerPrefs.GetFloat("FinalRight");
            blinkDura = PlayerPrefs.GetFloat("BlinkDura");

            if(PlayerPrefs.GetString("Mode") == "Manual")
            {
                manualBlink = true;
            }
        }

        void Update()
        {



            if (manualBlink == true)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    onBlink();
                }
            }
            else
            {
                //grabs the average values from the FaceProcessor and localises them to this script 
                currentLeftArea = 0;
                currentRightArea = 0;


                //print(currentLeftArea + " " + currentRightArea);

                if (faceDect.faceInCam())
                {


                    if (currentLeftArea < finalLeft && currentRightArea < finalRight && blinked == false)
                    {
                        StartCoroutine(cleanEye());

                    }


                }


            }
        }


            

        IEnumerator cleanEye()
        {
            print("called");
            blinked = true;

            float t = 0f;

            while (currentLeftArea < finalLeft && currentRightArea < finalRight)
            {
                t += Time.deltaTime;
                print(t + " " + blinkDura + " " + currentLeftArea);
                if (t > blinkDura)
                {
                    Debug.Log("BREAK - no blink" + currentLeftArea + " " + finalLeft);

                    blinked = false;
                    yield break;

                }
                yield return null;

            }


            if (currentLeftArea > finalLeft && currentRightArea > finalRight)
            {
                Debug.Log("BLINKED" + currentLeftArea);
                onBlink();
            }
            //yield return new WaitForSeconds(0.2f);
            yield return 0;
            blinked = false;
            yield break;

        }





    }

}