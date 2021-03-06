namespace OpenCvSharp.Demo
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.SceneManagement;
    using OpenCvSharp;
    using System;
    using System.IO;
    using System.Linq;


    public class CalibrationV2 : MonoBehaviour
    {
        [SerializeField]
        public bool printLeftList, printRightList;

        List<float> tempLeft = new List<float>();
        List<float> tempRight = new List<float>();
        List<float> sortedLeftEye = new List<float>();
        List<float> sortedRightEye = new List<float>();

        public Button startButt;
        public GameObject stage0, stage0h, stage1, stage2, stage2h, stage3, stage4;
        public GameObject faceDectSymbol, noFaceDectSymbol;
        public Slider timerScroll;



        public GameObject camHandle;

        public GameObject mainCanvas, WIPCanvas;

        public FaceDetectorScene faceDect;
        bool startCalibBool = false;
        bool calibrationSet = false;
        bool blinked = false;
        int time;
        float counter;
        int pointer = 0;
        float eyeNum;

        bool lastBlinkClosed = false;
        
        public float currentLeftArea, currentRightArea;

        float blinkDura = 0.15f;
        float threshholdMultiplier = 1.2f;

        int leftHalfPointer, rightHalfPointer;
        float finalLeft, finalRight;

        //[SerializeField, Range(0, 20)]
        public float blinkSensitivity = 0;

        int sixBlinksIndex = -1;
        public GameObject[] greenLights;

        public GameObject[] pointers;

        private void Start()
        {
            if (PlayerPrefs.GetString("Eye") == "Left")
                eyeNum = 1;
            else if (PlayerPrefs.GetString("Eye") == "Right")
                eyeNum = 2;
            else if (PlayerPrefs.GetString("Eye") == "Both")
                eyeNum = 3;
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

            if (stage0.activeInHierarchy)
            {
                if (faceDect.faceInCam())
                {
                    noFaceDectSymbol.SetActive(false);
                    faceDectSymbol.SetActive(true);
                    startButt.interactable = true;
                }
                else
                {
                    faceDectSymbol.SetActive(false);
                    noFaceDectSymbol.SetActive(true);
                    startButt.interactable = false;
                }
            }


            //activates blinked lights
            if (sixBlinksIndex >= 0 && sixBlinksIndex <= 5)
            {
                greenLights[sixBlinksIndex].SetActive(true);
                if (sixBlinksIndex == 5)
                {
                    stage2.SetActive(false);
                    stage4.SetActive(false);
                    stage3.SetActive(true);
                }
            }
            else if (sixBlinksIndex == -1)
            {
                for (int i = 0; i < greenLights.Length; i++)
                {
                    greenLights[i].SetActive(false);
                }
            }


            if (calibrationSet && faceDect.faceInCam())
            {

                //compares the current distance between the eyelids to the minimum distance before it's considered a blink
                finalLeft = sortedLeftEye[leftHalfPointer];
                finalRight = sortedRightEye[rightHalfPointer];

                if (eyeNum == 3)
                {
                    if (currentLeftArea < finalLeft && currentRightArea < finalRight && blinked == false)
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
                    if (currentRightArea < finalRight && blinked == false)
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


            //Calibration process
            if (startCalibBool)
            {
                counter += Time.deltaTime;


                if (counter < time)
                {
                    tempLeft.Add(currentLeftArea);
                    tempRight.Add(currentRightArea);

                    timerScroll.value = (counter / time) * 100;
                }
                else
                {
                    //Sorts the massive list by size
                    tempLeft.Sort(SortByBiggest);
                    tempRight.Sort(SortByBiggest);
                    //Sorts the sorted list from before by unique values
                    sortedLeftEye = tempLeft.Distinct().ToList();
                    sortedRightEye = tempRight.Distinct().ToList();
                    
                    
                    print("Timer over");

                    //SortByMost(tempLeft, sortedLeftEye);



                    SecondPartCalib();

                    print(calibrationSet);

                    //gets the bottom half of the sorted list
                    //grabs the last value on the list, which is the maximum distance it can be before it's considered a blink
                    //repeat for both eyes
                    leftHalfPointer = (int)((sortedLeftEye.Count() - 1) * 0.35f);
                    rightHalfPointer = (int)((sortedRightEye.Count() - 1) * 0.35f);



                    startCalibBool = false;



                }

            }

        }

        //Coroutine for when a blink is detected
        IEnumerator cleanLeftEye()
        {
            print("called left eye");
            blinked = true;


            yield return new WaitForSeconds(blinkDura);


            if (currentLeftArea > finalLeft)
            {
                Debug.Log("BLINKED" + currentLeftArea);
                sixBlinksIndex++;
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
                sixBlinksIndex++;
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
                sixBlinksIndex++;
            }
            else
            {
                Debug.Log("BREAK - no blink" + currentRightArea + " threshhold: " + finalRight);
                lastBlinkClosed = true;
            }


            blinked = false;
            yield break;

        }

        public void startCalibration()
        {
            
            stage0h.SetActive(false);
            stage1.SetActive(true);
            counter = 0;
            time = 5;
            startCalibBool = true;


        }

        //Sorting algorythm for the list sorter thing
        int SortByBiggest(float x, float y)
        {
            if (x == y)
            {
                return 0;
            }
            else if (x > y)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        public void MissedSome()
        {
            leftHalfPointer -= 1;
            blinkDura += 0.05f;
            threshholdMultiplier -= 0.05f;

            sixBlinksIndex = -1;
            stage3.SetActive(false);
            stage4.SetActive(true);
        }

        public void MissedALot()
        {
            leftHalfPointer -= 3;
            blinkDura += 0.1f;
            threshholdMultiplier -= 0.1f;

            sixBlinksIndex = -1;
            stage3.SetActive(false);
            stage4.SetActive(true);
        }

        public void ExtraBlinks()
        {
            leftHalfPointer += 1;
            threshholdMultiplier += 0.05f;

            sixBlinksIndex = -1;
            stage3.SetActive(false);
            stage4.SetActive(true);
        }

        public void ExtraBlinksPlus()
        {
            leftHalfPointer += 3;
            threshholdMultiplier += 0.1f;

            sixBlinksIndex = -1;
            stage3.SetActive(false);
            stage4.SetActive(true);
        }

        void SecondPartCalib()
        {
            calibrationSet = true;
            stage1.SetActive(false);
            stage2.SetActive(true);
            stage2h.SetActive(true);
        }

        public void ContinueButton()
        {
            mainCanvas.SetActive(false);
            WIPCanvas.SetActive(true);
        }

        public void StageZero()
        {
            stage0.SetActive(false);
            stage0h.SetActive(true);
        }
        
        public void StartButton()
        {
            PlayerPrefs.SetFloat("FinalLeft", finalLeft);
            PlayerPrefs.SetFloat("FinalRight", finalRight);
            PlayerPrefs.SetFloat("BlinkDura", blinkDura);
            PlayerPrefs.SetFloat("OpenEye", threshholdMultiplier);
            Destroy(camHandle);
            SceneManager.LoadScene(3);
        }



    }
}

