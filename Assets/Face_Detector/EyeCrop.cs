namespace OpenCvSharp.Demo
{
    using UnityEngine;
	using System.Collections;
	using OpenCvSharp;
	using UnityEngine.UI;
    
	public class EyeCrop : MonoBehaviour
    {

        public FaceDetectorScene faceDect;

        public RawImage outputRawimg;


        void Update()
        {
            //Mat mat = new Mat();
            //faceDect.boxMat().CopyTo(mat);

            //outputRawimg.transform.transform.localPosition = faceDect.boxMat();

            //Texture2D output;

            //output = Unity.MatToTexture(faceDect.copyMat());

            //outputRawimg.texture = output;


        }


    }

}


