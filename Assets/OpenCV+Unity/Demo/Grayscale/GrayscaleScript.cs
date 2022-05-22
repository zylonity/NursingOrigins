namespace OpenCvSharp.Demo
{
	using UnityEngine;
	using System.Collections;
	using OpenCvSharp;
	using UnityEngine.UI;
    
	public class GrayscaleScript : MonoBehaviour {

		public Texture2D texture;

		Color brown = new Color(233, 193, 189);
		float diff = 20;



		// Use this for initialization
		void Start () {
			float[] lowerBound = new float[] { brown.b - diff, brown.g - diff, brown.r - diff };
            float[] upperBound = new float[] { brown.b + diff, brown.g + diff, brown.r + diff };
			Mat mat = Unity.TextureToMat (this.texture);
			Mat processedMask = new Mat ();
			Mat finalMat = new Mat();
            
			//Filters out anything not in the range by turning it into 1s and 0s
			Cv2.InRange(mat, InputArray.Create(lowerBound), InputArray.Create(upperBound), processedMask);

            //Adds colour to the bit image thing I made
            Cv2.BitwiseAnd(mat, mat, finalMat, processedMask);

            int picSize = mat.Size().Width * mat.Size().Height;
            int pixelCount = Cv2.CountNonZero(processedMask);

            float pixelPercentage = ((float)pixelCount / (float)picSize) * 100;

            print(pixelPercentage);

            Texture2D texture = Unity.MatToTexture (finalMat);

			RawImage rawImage = gameObject.GetComponent<RawImage> ();
			rawImage.texture = texture;
	//		Renderer renderer = gameObject.GetComponent<Renderer> ();
	//		renderer.material.mainTexture = texture;
		}

		// Update is called once per frame
		void Update () {

		}


	}
}