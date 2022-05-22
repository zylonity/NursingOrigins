namespace OpenCvSharp.Demo
{
    using UnityEngine;
	using System.Collections;
	using OpenCvSharp;
	using UnityEngine.UI;
    
	public class EyeCrop : MonoBehaviour
    {

        public FaceDetectorScene faceDect;
        public int eyeNumber;

        public RawImage rawimg;

        Color boundary = new Color(79, 65, 75);
        float diff = 30;

        float[] lowerBound;
        float[] upperBound;


        void Start()
        {
            lowerBound = new float[] { boundary.b - diff, boundary.g - diff, boundary.r - diff };
            upperBound = new float[] { boundary.b + diff, boundary.g + diff, boundary.r + diff };

        }


        void Update()
        {
            Mat mat = new Mat();
            faceDect.boxMat(eyeNumber).CopyTo(mat);
            Mat processedMask = new Mat();
            Mat finalMat = new Mat();

            Texture2D output;

            //Filters out anything not in the range by turning it into 1s and 0s
            Cv2.InRange(mat, InputArray.Create(lowerBound), InputArray.Create(upperBound), processedMask);

            //Adds colour to the bit image thing I made
            Cv2.BitwiseAnd(mat, mat, finalMat, processedMask);

            int picSize = mat.Size().Width * mat.Size().Height;
            int pixelCount = Cv2.CountNonZero(processedMask);

            float pixelPercentage = ((float)pixelCount / (float)picSize) * 100;

            //print(pixelPercentage);
            print(mat.Size().Width * mat.Size().Height);


            //output
            output = Unity.MatToTexture(processedMask);

            rawimg.texture = output;
            //RawImage rawImage = gameObject.GetComponent<RawImage>();
            //rawImage.texture = texture;
            //		Renderer renderer = gameObject.GetComponent<Renderer> ();
            //		renderer.material.mainTexture = texture;
        }


    }

}


