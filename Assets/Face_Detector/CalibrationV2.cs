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

        public float currentLeftArea, currentRightArea;

        float blinkDura = 0.15f;
        int leftHalfPointer, rightHalfPointer;
        float finalLeft, finalRight;

        //[SerializeField, Range(0, 20)]
        public float blinkSensitivity = 0;

        int sixBlinksIndex = -1;
        public GameObject[] greenLights;

        public GameObject[] pointers;

        

        void Update()
        {

            //grabs the average values from the FaceProcessor and localises them to this script 
            currentLeftArea = faceDect.EyeArea(6);
            currentRightArea = faceDect.EyeArea(5);

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



                //print(faceDect.EyeArea(6) + " " + sortedLeftEye[leftHalfPointer]);
                if (currentLeftArea < finalLeft && currentRightArea < finalRight && blinked == false)
                {
                    //blinked = true;
                    print((sortedLeftEye.Count() - 1) + " " + (sortedRightEye.Count() - 1));
                    StartCoroutine(cleanEye());

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

                    if (counter < 3)
                    {
                        pointers[0].SetActive(true);
                        timerScroll.value = counter / 3 * 100;
                    }
                    else if (counter > 3 && counter < 6)
                    {
                        pointers[0].SetActive(false);
                        pointers[1].SetActive(true);
                        timerScroll.value = ((counter - 3) / 3) * 100;
                    }
                    else if (counter > 6 && counter < 9)
                    {
                        pointers[1].SetActive(false);
                        pointers[2].SetActive(true);
                        timerScroll.value = ((counter - 6) / 3) * 100;
                    }
                    else if (counter > 9 && counter < 12)
                    {
                        pointers[2].SetActive(false);
                        pointers[3].SetActive(true);
                        timerScroll.value = ((counter - 9) / 3) * 100;
                    }
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
                    Debug.Log("BREAK - no blink");

                    blinked = false;
                    yield break;

                }
                yield return null;

            }


            if (currentLeftArea > finalLeft && currentRightArea > finalRight)
            {
                Debug.Log("BLINKED" + " " + finalLeft);
                sixBlinksIndex++;
            }
            //yield return new WaitForSeconds(0.2f);
            yield return 0;
            blinked = false;
            yield break;

        }

        public void startCalibration()
        {
            
            stage0h.SetActive(false);
            stage1.SetActive(true);
            counter = 0;
            time = 12;
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

            sixBlinksIndex = -1;
            stage3.SetActive(false);
            stage4.SetActive(true);
        }

        public void MissedALot()
        {
            leftHalfPointer -= 3;
            blinkDura += 0.1f;

            sixBlinksIndex = -1;
            stage3.SetActive(false);
            stage4.SetActive(true);
        }

        public void ExtraBlinks()
        {
            leftHalfPointer += 1;

            sixBlinksIndex = -1;
            stage3.SetActive(false);
            stage4.SetActive(true);
        }

        public void ExtraBlinksPlus()
        {
            leftHalfPointer += 3;

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
            Destroy(camHandle);
            SceneManager.LoadScene(2);
        }



    }
}

