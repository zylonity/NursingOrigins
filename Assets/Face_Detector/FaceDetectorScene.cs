namespace OpenCvSharp.Demo
{
	using System;
	using UnityEngine;
	using System.Collections.Generic;
	using UnityEngine.UI;
	using OpenCvSharp;
	using System.Linq;

	public class FaceDetectorScene : WebCamera
	{
		public float leftEyeAverage, rightEyeAverage;

		public TextAsset faces;
		public TextAsset eyes;
		public TextAsset shapes;

		public bool inGame = false;

		private FaceProcessorLive<WebCamTexture> processor;

		/// <summary>
		/// Default initializer for MonoBehavior sub-classes
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			base.forceFrontalCamera = true; // we work with frontal cams here, let's force it for macOS s MacBook doesn't state frontal cam correctly

			byte[] shapeDat = shapes.bytes;
			if (shapeDat.Length == 0)
			{
				string errorMessage =
					"In order to have Face Landmarks working you must download special pre-trained shape predictor " +
					"available for free via DLib library website and replace a placeholder file located at " +
					"\"OpenCV+Unity/Assets/Resources/shape_predictor_68_face_landmarks.bytes\"\n\n" +
					"Without shape predictor demo will only detect face rects.";

#if UNITY_EDITOR
				// query user to download the proper shape predictor
				if (UnityEditor.EditorUtility.DisplayDialog("Shape predictor data missing", errorMessage, "Download", "OK, process with face rects only"))
					Application.OpenURL("http://dlib.net/files/shape_predictor_68_face_landmarks.dat.bz2");
#else
             UnityEngine.Debug.Log(errorMessage);
#endif
			}

			processor = new FaceProcessorLive<WebCamTexture>();
			processor.Initialize(faces.text, eyes.text, shapes.bytes);

			// data stabilizer - affects face rects, face landmarks etc.
			processor.DataStabilizer.Enabled = false;        // enable stabilizer
			processor.DataStabilizer.Threshold = 1.0;       // threshold value in pixels
			processor.DataStabilizer.SamplesCount = 2;      // how many samples do we need to compute stable data

			// performance data - some tricks to make it work faster
			processor.Performance.Downscale = 256;          // processed image is pre-scaled down to N px by long side
			processor.Performance.SkipRate = 0;             // we actually process only each Nth frame (and every frame for skipRate = 0)





		}

		/// <summary>
		/// Per-frame video capture processor
		/// </summary>
		protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
		{


            // detect everything we're interested in
            processor.ProcessTexture(input, TextureParameters);

            if (!inGame)
            {
				// mark detected objects
				processor.MarkDetected();

				// processor.Image now holds data we'd like to visualize
				output = Unity.MatToTexture(processor.Image, output);   // if output is valid texture it's buffer will be re-used, otherwise it will be re-created
			}


			return true;
		}

		public bool faceInCam()
		{
			if (processor.Faces.Count == 1)
			{
				return true;
			}
			return false;
		}



		public float EyeArea(int eye)
		{
			//TODO mess with this
			if (processor.Faces.Count >= 1)
			{
				DetectedFace face = processor.Faces[0];

				//Calcuate area for left eye from the given points
				List<Point> eyePoints = face.Elements[eye].Marks.ToList();
				eyePoints.Add(eyePoints[0]);

				float Area = 0;
				for (int i = 0; i < eyePoints.Count - 1; i++)
				{
					Area += (eyePoints[i + 1].X - eyePoints[i].X) * (eyePoints[i + 1].Y + eyePoints[i].Y) / 2;
				}



				return -Area;
			}
			else
			{
				return 0;

			}


		}




	}


}