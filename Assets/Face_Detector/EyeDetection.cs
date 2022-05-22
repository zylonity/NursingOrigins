namespace OpenCvSharp.Demo
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using OpenCvSharp;
    using System.IO;
    using System.Linq;


    public class EyeDetection : MonoBehaviour
    {
        [SerializeField]
        public bool printLeftList, printRightList;

        //public TextAsset textfile;

        float[] leftEyeVals = new float[50];
        float[] rightEyeVals = new float[50];

        List<float> openLeftEye = new List<float>();
        List<float> openRightEye = new List<float>();
        List<float> closedLeftEye = new List<float>();
        List<float> closedRightEye = new List<float>();

        public GameObject blackScreen;
        public GameObject openEyesButton, closedEyesButton, tooLittleBlinksButton, tooManyBlinksButton;
        
        public FaceDetectorScene faceDect;
        bool timer = false;
        bool calibrationSet = false;
        bool blinked = false;
        int time;
        int calibrationMode;
        float counter;

        float avg;
        float openLeftAvg, openRightAvg;
        float closedLeftAvg, closedRightAvg;
        public float currentLeftPos, currentRightPos;
        bool firstArraysSet = false;

        bool calibOpen = false;
        bool calibClosed = false;

        //[SerializeField, Range(0, 20)]
        public float blinkSensitivity = 0;


        void Update()
        {
            currentLeftPos = faceDect.FaceDetected().Item1;
            currentRightPos = faceDect.FaceDetected().Item2;


            if (calibrationSet)
            {
                int x = 0;
                if (x == 49)
                {
                    x = 0;
                }
                leftEyeVals[x] = currentLeftPos;
                rightEyeVals[x] = currentRightPos;


                float left4;
                float right4;
                if (x == 0)
                {
                    left4 = (leftEyeVals[x] + leftEyeVals[49] + leftEyeVals[48] + leftEyeVals[47]);
                    right4 = (rightEyeVals[x] + rightEyeVals[49] + rightEyeVals[48] + rightEyeVals[47]);
                }
                else if (x == 1)
                {
                    left4 = (leftEyeVals[x] + leftEyeVals[0] + leftEyeVals[49] + leftEyeVals[48]);
                    right4 = (rightEyeVals[x] + rightEyeVals[0] + rightEyeVals[49] + rightEyeVals[48]);
                }
                else if (x == 2)
                {
                    left4 = (leftEyeVals[x] + leftEyeVals[1] + leftEyeVals[0] + leftEyeVals[49]);
                    right4 = (rightEyeVals[x] + rightEyeVals[1] + rightEyeVals[0] + rightEyeVals[49]);
                }
                else
                {
                    left4 = (leftEyeVals[x] + leftEyeVals[x - 1] + leftEyeVals[x - 2] + leftEyeVals[x - 3]);
                    right4 = (rightEyeVals[x] + rightEyeVals[x - 1] + rightEyeVals[x - 2] + rightEyeVals[x - 3]);
                }


                if (left4 < closedLeftAvg + blinkSensitivity && blinked == false)
                {
                    StartCoroutine(hasBlinked());

                }

                x++;
            }


            //Calibration process
            if (timer)
            {
                counter += Time.deltaTime;
                if (counter <= time)
                {
                    if (faceDect.faceInCam())
                    {
                        if (calibrationMode == 1)
                        {
                            openLeftEye.Add(currentLeftPos);
                            openRightEye.Add(currentRightPos);
                        }
                        else if (calibrationMode == 2)
                        {
                            closedLeftEye.Add(currentLeftPos);
                            closedRightEye.Add(currentRightPos);
                        }

                    }
                }
                else
                {
                    if (calibrationMode == 1)
                    {
                        openLeftAvg = AverageEye(openLeftEye);
                        openRightAvg = AverageEye(openRightEye);
                        calibOpen = true;
                    }
                    else if (calibrationMode == 2)
                    {
                        closedLeftAvg = AverageEye(closedLeftEye);
                        closedRightAvg = AverageEye(closedRightEye);
                        calibClosed = true;
                    }                    

                    //print(openLeftAvg);
                    print("Timer over");


                    if (calibOpen && calibClosed)
                    {
                        SecondPartCalib();

                    }
                    print(calibrationSet);
                    timer = false;
                }
            }

        }

        IEnumerator hasBlinked()
        {
            blinked = true;
            blackScreen.SetActive(!blackScreen.activeInHierarchy);
            yield return new WaitForSeconds(0.2f);
            blinked = false;

        }


        public void CalibrateOpenEyes()
        {
            ClearAll();
            print("Keep your eyes OPEN for 3 seconds, look naturally at your monitor, do not move around too much from the distance you're currently at");
            calibrationMode = 1;
            counter = 0;
            timer = true;
            time = 3;
            
        }
        
        public void CalibrateClosedEyes()
        {
            ClearAll();
            print("Keep your eyes CLOSED for 3 seconds");
            calibrationMode = 2;
            counter = 0;
            timer = true;
            time = 3;
        }

        public void tooLittleDetected()
        {
            blinkSensitivity += 0.3f;
        }

        public void tooManyDetected()
        {
            blinkSensitivity -= 0.3f;
        }

        void SecondPartCalib()
        {
            calibrationSet = true;
            openEyesButton.SetActive(false);
            closedEyesButton.SetActive(false);
            tooLittleBlinksButton.SetActive(true);
            tooManyBlinksButton.SetActive(true);
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


        void WriteString(List<float> leftie, List<float> righty, int mode)
        {

            string path = "Assets/test.txt";

            StreamWriter writer = new StreamWriter(path, true);

            if(mode == 0)
            {
                for (int i = 0; i < leftie.Count; i++)
                {
                    writer.WriteLine("Left: " + leftie[i]);
                    writer.WriteLine("Right: " + righty[i]);
                }
            }
            writer.Close();

        }

        void ClearAll()
        {
            File.WriteAllText("Assets/test.txt", string.Empty);
        }

        private void OnApplicationQuit()
        {
            //WriteString(initialLeftEye, initialRightEye, 0);
        }



    }
}

