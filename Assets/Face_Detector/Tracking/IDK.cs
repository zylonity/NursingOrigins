namespace OpenCvSharp.Demo
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using OpenCvSharp;
    using UnityEngine.UI;

    public class IDK : MonoBehaviour
    {

        WebCamTexture webCamText;

        public RawImage rawimg;

        void Start()
        {
            WebCamDevice[] devices = WebCamTexture.devices;
            for (int i = 0; i < devices.Length; i++)
                Debug.Log(devices[i].name);

            webCamText = new WebCamTexture(devices[0].name);
            //webCamText.deviceName = devices[0].name;
            webCamText.Play();
        }

        void Update()
        {
            Mat mat = Unity.TextureToMat(webCamText);

            int firstFaceX = (int)(mat.Width * 0.38f);
            int firstFaceY = (int)(mat.Height * 0.2f);
            int lengthFaceX = (int)(mat.Width * 0.62f) - firstFaceX;
            int lengthFaceY = (int)(mat.Height * 0.60f) - firstFaceY;


            OpenCvSharp.Rect tempRect = new OpenCvSharp.Rect(firstFaceX, firstFaceY, lengthFaceX, lengthFaceY);

            Cv2.Rectangle(mat, tempRect, Scalar.FromRgb(255, 255, 0), 3);

            //OpenCvSharp.Rect crop = new OpenCvSharp.Rect(100, 100, 200, 100);
            Mat croppedMat = new Mat(mat, tempRect);

            Texture2D output;

            //output
            output = Unity.MatToTexture(mat);

            rawimg.texture = output;
        }
    }
}

