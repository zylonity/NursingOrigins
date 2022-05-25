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
    using System.Threading;

    public class BlinkDect : MonoBehaviour
    {

        public delegate void Blinked();
        public static event Blinked onBlink;


        bool manualBlink = false;

        public GameObject camHandle;

        public FaceDetectorScene faceDect;
        bool blinked = false;
        bool lastBlinkClosed;

        public float currentLeftArea, currentRightArea;

        float finalLeft, finalRight;
        float blinkDura;
        float threshholdMultiplier;

        int eyeNum;



        void Start()
        {
            finalLeft = PlayerPrefs.GetFloat("FinalLeft");
            finalRight = PlayerPrefs.GetFloat("FinalRight");
            blinkDura = PlayerPrefs.GetFloat("BlinkDura");
            threshholdMultiplier = PlayerPrefs.GetFloat("OpenEye");


            if (PlayerPrefs.GetString("Eye") == "Left")
                eyeNum = 1;
            else if (PlayerPrefs.GetString("Eye") == "Right")
                eyeNum = 2;
            else if (PlayerPrefs.GetString("Eye") == "Both")
                eyeNum = 3;
            else if (PlayerPrefs.GetString("Eye") == "Manual")
                manualBlink = true;
            else
                eyeNum = 0;




        }

        void Update()
        {
            if (eyeNum == 3)
            {
                currentLeftArea = faceDect.EyeArea(6);
                currentRightArea = faceDect.EyeArea(5);
            }
            else if (eyeNum == 2)
            {
                currentRightArea = faceDect.EyeArea(5);
            }
            else if (eyeNum == 1)
            {
                currentLeftArea = faceDect.EyeArea(6);
            }

            if (manualBlink == true)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    onBlink();
                }
            }
            else
            {

                if (faceDect.faceInCam() && faceDect.processingC)
                {

                    print(currentLeftArea + currentRightArea);
                    //grabs the average values from the FaceProcessor and localises them to this script 
                    if (eyeNum == 3)
                    {
                        if (currentLeftArea < finalLeft && currentRightArea < finalRight && blinked == false && lastBlinkClosed == false)
                        {
                            StartCoroutine(cleanEyes());
                        }

                        if (lastBlinkClosed && currentLeftArea > finalLeft * threshholdMultiplier && currentRightArea > finalRight * threshholdMultiplier)
                        {
                            lastBlinkClosed = false;
                            print("Last blink over");
                        }

                    }
                    else if (eyeNum == 2)
                    {
                        if (currentRightArea < finalRight && blinked == false && lastBlinkClosed == false)
                        {
                            StartCoroutine(cleanRightEye());

                        }

                        if (lastBlinkClosed && currentLeftArea > finalLeft * threshholdMultiplier)
                        {
                            lastBlinkClosed = false;
                            print("Last blink over");
                        }


                    }
                    else if (eyeNum == 1)
                    {
                        if (currentLeftArea < finalLeft && blinked == false && lastBlinkClosed == false)
                        {
                            StartCoroutine(cleanLeftEye());

                        }


                        if (lastBlinkClosed && currentLeftArea > finalLeft * threshholdMultiplier)
                        {
                            lastBlinkClosed = false;
                            print("Last blink over");
                        }


                    }
                }
                

            }
        }



        IEnumerator cleanLeftEye()
        {
            print("called left eye");
            blinked = true;


            yield return new WaitForSeconds(blinkDura);


            if (currentLeftArea > finalLeft)
            {
                Debug.Log("BLINKED" + currentLeftArea);
                onBlink();
            }
            else
            {
                Debug.Log("BREAK - no blink" + currentLeftArea + " " + finalLeft);
                lastBlinkClosed = true;
            }


            blinked = false;
            yield break;

        }

        IEnumerator cleanEyes()
        {
            print("called both eyes");
            blinked = true;

            yield return new WaitForSeconds(blinkDura);

            if (currentLeftArea > finalLeft && currentRightArea > finalRight)
            {
                print("BLINKED" + currentLeftArea + " " + currentRightArea);
                onBlink();
            }
            else
            {
                Debug.Log("BREAK - no blink" + currentLeftArea + " threshhold: " + finalLeft + "  " + currentRightArea + " threshhold: " + finalRight);
                lastBlinkClosed = true;
            }


            blinked = false;
            yield break;

        }

        IEnumerator cleanRightEye()
        {
            print("called right eye");
            blinked = true;

            yield return new WaitForSeconds(blinkDura);

            if (currentRightArea > finalRight)
            {
                print("BLINKED" + currentRightArea);
                onBlink();
            }
            else
            {
                Debug.Log("BREAK - no blink" + currentRightArea + " threshhold: " + finalRight);
                lastBlinkClosed = true;
            }


            blinked = false;
            yield break;

        }


    }

}