namespace OpenCvSharp.Demo
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using OpenCvSharp;
    using System.IO;
    using System.Linq;


    public class CalibrationV3 : MonoBehaviour
    {
        [SerializeField]
        public bool printLeftList, printRightList;

        //public TextAsset textfile;

        List<float> leftEyeVals = new List<float>();
        List<float> rightEyeVals = new List<float>();

        List<float> tempLeft = new List<float>();
        List<float> tempRight = new List<float>();
        List<float> sortedLeftEye = new List<float>();
        List<float> sortedRightEye = new List<float>();

        float[] calibratedLeftBlink = new float[11];
        float[] calibratedRightBlink = new float[11];

        public Button startButt;
        public GameObject stage0, stage0h, stage1, stage2, stage2h, stage3, stage4;
        public GameObject faceDectSymbol, noFaceDectSymbol;
        public Slider timerScroll;

        public GameObject mainCanvas, WIPCanvas;

        public FaceDetectorScene faceDect;
        bool startCalibBool = false;
        bool calibrationSet = false;
        bool blinked = false;
        int time;
        float counter;
        int pointer = 0;

        float avg;
        float currentLeftAvg, currentRightAvg;
        public float currentLeftPos, currentRightPos;


        //[SerializeField, Range(0, 20)]
        public float blinkSensitivity = 0;

        int sixBlinksIndex = -1;
        public GameObject[] greenLights;




        void Update()
        {

            //grabs the average values from the FaceProcessor and localises them to this script 
            currentLeftPos = faceDect.FaceDetected().Item1;
            currentRightPos = faceDect.FaceDetected().Item2;

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

                leftEyeVals.Add(currentLeftPos);
                rightEyeVals.Add(currentRightPos);
                
                if (leftEyeVals.Count == 9)
                    leftEyeVals.RemoveAt(0);

                if (rightEyeVals.Count == 9)
                    rightEyeVals.RemoveAt(0);


                
                currentLeftAvg = Queryable.Average(leftEyeVals.AsQueryable());
                currentRightAvg = Queryable.Average(rightEyeVals.AsQueryable());


                //gets the bottom half of the sorted list
                //grabs the last value on the list, which is the maximum distance it can be before it's considered a blink
                //repeat for both eyes
                List<float> lowerHalfLeftEye = LowerHalfOfList(sortedLeftEye);
                int maxLeftHalf = lowerHalfLeftEye.Count - 1;

                //right eye
                List<float> lowerHalfRightEye = LowerHalfOfList(sortedRightEye);
                int maxRightHalf = lowerHalfRightEye.Count - 1;


                float[] lInitialBlink = new float[4];
                float[] lAfterBlink = new float[4];
                float[] rInitialBlink = new float[4];
                float[] rAfterBlink = new float[4];


                for (int j = 0; j < 4; j++)
                {
                    lInitialBlink[j] = leftEyeVals[j];
                    rInitialBlink[j] = rightEyeVals[j];

                    lAfterBlink[j] = leftEyeVals[j + 4];
                    rAfterBlink[j] = rightEyeVals[j + 4];
                }

                float lTopRange = lAfterBlink[3] - lAfterBlink[0];
                float lBottomRange = lInitialBlink[0] - lInitialBlink[3];

                float rTopRange = rAfterBlink[3] - rAfterBlink[0];
                float rBottomRange = rInitialBlink[0] - rInitialBlink[3];

                float lCalibTopR = (calibratedLeftBlink[8] - calibratedLeftBlink[5]) - blinkSensitivity;
                float lCalibBottomR = (calibratedLeftBlink[0] - calibratedLeftBlink[3]) - blinkSensitivity;

                float rCalibTopR = (calibratedRightBlink[8] - calibratedRightBlink[5]) - blinkSensitivity;
                float rCalibBottomR = (calibratedRightBlink[0] - calibratedRightBlink[3]) - blinkSensitivity;



                //print(leftEyeVals[4] + " " + sortedLeftEye[maxLeftHalf]);
                //compares the current distance between the eyelids to the minimum distance before it's considered a blink
                if (lBottomRange > lCalibBottomR && rBottomRange > rCalibBottomR&& blinked == false)
                {

                    print("Calibrated right bottom range: " + (rCalibBottomR) + " right bottom range: " + (rBottomRange ));
                    print("right top range: " + rCalibTopR + " right bottom range: " + rTopRange);

                    if (lTopRange > lCalibTopR  && rTopRange > rCalibTopR)
                    {
                        print("Calibrated right top range: " + (rCalibTopR) + " right top range: " + (rTopRange));
                        StartCoroutine(hasBlinked());
                    }
                    

                    //StartCoroutine(hasBlinked());

                }
                
            }


            //Calibration process
            if (startCalibBool)
            {
                
                counter += Time.deltaTime;

                if (counter < time)
                {
                    tempLeft.Add(currentLeftPos);
                    tempRight.Add(currentRightPos);

                    timerScroll.value = (counter / time) * 100;



                }
                else
                {

                    int indexOfLeftMin = tempLeft.IndexOf(tempLeft.Min());
                    int indexOfRightMin = tempRight.IndexOf(tempRight.Min());


                    //Gets one of the blinks from the calibration and sets it to an array (CAN IMPROVE AUTOMATIC ACCURACY IF I DETECT MORE THAN ONE)
                    for (int k = 0; k < 9; k++)
                    {
                        calibratedLeftBlink[k] = tempLeft[indexOfLeftMin - (4 - k)];
                        calibratedRightBlink[k] = tempRight[indexOfRightMin - (4 - k)];
                    }
                    
                    //Sorts the massive list by size
                    tempLeft.Sort(SortByBiggest);
                    tempRight.Sort(SortByBiggest);
                    //Sorts the sorted list from before by unique values
                    sortedLeftEye = tempLeft.Distinct().ToList();
                    sortedRightEye = tempRight.Distinct().ToList();


                    print("Timer over");

                    SortByMost(tempLeft, sortedLeftEye);

                    SecondPartCalib();

                    //print(calibrationSet);
                    startCalibBool = false;
                }



                //counter += Time.deltaTime;
                //if (counter <= time)
                //{
                //    timerScroll.value = (counter / time) * 100;

                //    if (faceDect.faceInCam())
                //    {



                //    }
                //}
                //else
                //{

                //    //ADD anything for after the list is done here

                //}
            }

        }

        //Coroutine for when a blink is detected
        IEnumerator hasBlinked()
        {
            blinked = true;
            sixBlinksIndex++;
            //print(sixBlinksIndex);
            yield return new WaitForSeconds(0.2f);
            blinked = false;

        }




        public void startCalibration()
        {

            stage0h.SetActive(false);
            stage1.SetActive(true);
            print("Blink naturally for the next 5 seconds");
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

        //Grabs the lower half of the given list
        List<float> LowerHalfOfList(List<float> list)
        {
            List<float> halfList = new List<float>();

            for (int i = 0; i < (list.Count / 2); i++)
            {
                halfList.Add(list[i]);
            }

            return halfList;
        }

        //Grabs the upper half of the given list
        List<float> UpperHalfOfList(List<float> list)
        {
            List<float> halfList = new List<float>();

            for (int i = (list.Count / 2); i < list.Count; i++)
            {
                halfList.Add(list[i]);
            }

            return halfList;
        }


        //Counts list by the ammount of number something shows up, and prints out a dictionary
        void SortByMost(List<float> oldList, List<float> filteredList)
        {
            Dictionary<float, int> ammountPerNumber = new Dictionary<float, int>();

            foreach (float x in filteredList)
            {
                ammountPerNumber.Add(x, 0);
            }

            foreach (float j in oldList)
            {
                if (ammountPerNumber.ContainsKey(j))
                {
                    ammountPerNumber[j]++;
                }
            }

            foreach (float x in ammountPerNumber.Keys)
            {
                //print(x + "value is: " + ammountPerNumber[x]);
            }

        }

        public void tooLittleDetected()
        {
            blinkSensitivity += 0.5f;
            sixBlinksIndex = -1;
            //stage3.SetActive(false);
            //stage4.SetActive(true);
        }

        public void tooManyDetected()
        {
            blinkSensitivity -= 0.5f;
            sixBlinksIndex = -1;
            //stage3.SetActive(false);
            //stage4.SetActive(true);
        }

        void SecondPartCalib()
        {
            calibrationSet = true;
            stage1.SetActive(false);
            stage2.SetActive(true);
            stage2h.SetActive(true);
        }

        //calculate average of given list
        float AverageEye(List<float> givenList)
        {
            for (int i = 0; i < givenList.Count; i++)
            {
                avg += givenList[i];
            }
            avg = avg / givenList.Count;
            return avg;
        }

        float OpenCloseRange(List<float> openEyeList, List<float> closedEyeList)
        {
            return openEyeList.Min() - closedEyeList.Max();
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


        //void WriteString(List<float> leftie, List<float> righty, int mode)
        //{

        //    string path = "Assets/test.txt";

        //    StreamWriter writer = new StreamWriter(path, true);

        //    if (mode == 0)
        //    {
        //        for (int i = 0; i < leftie.Count; i++)
        //        {
        //            writer.WriteLine("Lower: " + leftie[i]);
        //        }
        //        for (int i = 0; i < leftie.Count; i++)
        //        {
        //            writer.WriteLine("Upper: " + righty[i]);
        //        }
        //    }
        //    writer.Close();

        //}

        //void ClearAll()
        //{
        //    File.WriteAllText("Assets/test.txt", string.Empty);
        //}

        //private void OnApplicationQuit()
        //{

        //    WriteString(LowerHalfOfList(sortedLeftEye), UpperHalfOfList(sortedLeftEye), 0);
        //}



    }
}

