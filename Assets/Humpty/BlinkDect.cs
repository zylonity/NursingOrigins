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
    
        public float currentLeftArea, currentRightArea;

        float finalLeft, finalRight;
        float blinkDura;

        int eyeNum;



        void Start()
        {
            finalLeft = PlayerPrefs.GetFloat("FinalLeft");
            finalRight = PlayerPrefs.GetFloat("FinalRight");
            blinkDura = PlayerPrefs.GetFloat("BlinkDura");

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
                if (eyeNum == 3)
                {
                    currentLeftArea = faceDect.EyeArea(6);
                    currentRightArea = faceDect.EyeArea(5);
                    if (faceDect.faceInCam() && currentLeftArea < finalLeft && currentRightArea < finalRight && blinked == false)
                    {
                        StartCoroutine(cleanEyes());
                    }
                }
                else if (eyeNum == 2)
                {
                    currentRightArea = faceDect.EyeArea(5);
                    if (faceDect.faceInCam() && currentRightArea < finalRight && blinked == false)
                    {
                        StartCoroutine(cleanRightEye());

                    }
                }
                else if (eyeNum == 1)
                {
                    currentLeftArea = faceDect.EyeArea(6);
                    if (faceDect.faceInCam() && currentLeftArea < finalLeft && blinked == false)
                    {
                        StartCoroutine(cleanLeftEye());

                    }
                }

            }
        }


            

        IEnumerator cleanLeftEye()
        {
            print("called left eye");
            blinked = true;

            float t = 0f;

            while (currentLeftArea < finalLeft)
            {
                t += Time.deltaTime;
                if (t > blinkDura)
                {
                    Debug.Log("BREAK - no blink" + currentLeftArea + " " + finalLeft);

                    blinked = false;
                    yield break;

                }
                yield return null;

            }


            if (currentLeftArea > finalLeft)
            {
                Debug.Log("BLINKED" + currentLeftArea);
                onBlink();
            }

            
            yield return 0;
            blinked = false;
            yield break;

        }

        IEnumerator cleanEyes()
        {
            print("called both eyes");
            blinked = true;

            float t = 0f;

            while (currentLeftArea < finalLeft && currentRightArea < finalRight)
            {
                t += Time.deltaTime;
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
                print("BLINKED" + currentLeftArea);
                onBlink();
            }

            
            yield return 0;
            blinked = false;
            yield break;

        }

        IEnumerator cleanRightEye()
        {
            print("called");
            blinked = true;

            float t = 0f;

            while (currentRightArea < finalRight)
            {
                t += Time.deltaTime;
                print(t + " " + blinkDura + " " + currentLeftArea);
                if (t > blinkDura)
                {
                    print("BREAK - no blink" + currentRightArea + " " + finalRight);

                    blinked = false;
                    yield break;

                }
                yield return null;

            }


            if (currentRightArea > finalRight)
            {
                print("BLINKED" + currentRightArea);
                onBlink();
            }
            //yield return new WaitForSeconds(0.2f);
            yield return 0;
            blinked = false;
            yield break;

        }

    }

}