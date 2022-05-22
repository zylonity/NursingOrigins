namespace OpenCvSharp.Demo
{
	using UnityEngine;
	using System.Collections;
	using OpenCvSharp;
	using UnityEngine.UI;
    
	public class ColourMask : MonoBehaviour {

		//public Texture2D texture;
		private WebCamTexture webCamText;

		public RawImage rawimg;

		Color boundary = new Color(164, 145, 160);
		float diff = 20;

		float[] lowerBound;
		float[] upperBound;


		void Start () {
			lowerBound = new float[] { boundary.b - diff, boundary.g - diff, boundary.r - diff };
			upperBound = new float[] { boundary.b + diff, boundary.g + diff, boundary.r + diff };

			webCamText = new WebCamTexture();
			webCamText.Play();
		}


		void Update () {
			Mat mat = Unity.TextureToMat(webCamText);
            Mat processedMask = new Mat();
            Mat finalMat = new Mat();

            OpenCvSharp.Rect crop = new OpenCvSharp.Rect(100, 100, 200, 100);
            Mat croppedMat = new Mat(mat, crop);

            Texture2D output;

            //Filters out anything not in the range by turning it into 1s and 0s
            Cv2.InRange(croppedMat, InputArray.Create(lowerBound), InputArray.Create(upperBound), processedMask);

            //Adds colour to the bit image thing I made
            //Cv2.BitwiseAnd(mat, mat, finalMat, processedMask);

            //int picSize = croppedMat.Size().Width * croppedMat.Size().Height;
            //int pixelCount = Cv2.CountNonZero(processedMask);

            //float pixelPercentage = ((float)pixelCount / (float)picSize) * 100;

            //print(pixelPercentage);



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