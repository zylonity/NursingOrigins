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

    public class EyeProcessing : MonoBehaviour
    {
        public FaceDetectorScene faceDect;


        void Update()
        {
            print(faceDect.EyeArea(6) + " " + faceDect.EyeArea(5));
        }
    }
}


