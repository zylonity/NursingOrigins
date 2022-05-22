namespace OpenCvSharp.Demo
{
	using System;
	using UnityEngine;
	using System.Collections.Generic;
	using UnityEngine.UI;
	using OpenCvSharp;

	public class FaceDetectorScene : WebCamera
	{
		public float leftEyeAverage, rightEyeAverage;

		public TextAsset faces;
		public TextAsset eyes;
		public TextAsset shapes;

		private FaceProcessorLive<WebCamTexture> processor;




		/// <summary>
		/// Default initializer for MonoBehavior sub-classes
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			base.forceFrontalCamera = true; // we work with frontal cams here, let's force it for macOS s MacBook doesn't state frontal cam correctly

			byte[] shapeDat = shapes.bytes;

			processor = new FaceProcessorLive<WebCamTexture>();
			processor.Initialize(faces.text, eyes.text, shapes.bytes);

			// data stabilizer - affects face rects, face landmarks etc.
			processor.DataStabilizer.Enabled = false;        // enable stabilizer
			processor.DataStabilizer.Threshold = 2.0;       // threshold value in pixels
			processor.DataStabilizer.SamplesCount = 2;      // how many samples do we need to compute stable data

			// performance data - some tricks to make it work faster
			processor.Performance.Downscale = 852;          // processed image is pre-scaled down to N px by long side
			processor.Performance.SkipRate = 0;             // we actually process only each Nth frame (and every frame for skipRate = 0)





		}

		/// <summary>
		/// Per-frame video capture processor
		/// </summary>
		protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
		{


            // detect everything we're interested in
            processor.ProcessTexture(input, TextureParameters);

			// mark detected objects
			processor.MarkDetected();

			// processor.Image now holds data we'd like to visualize
			output = Unity.MatToTexture(processor.Image, output);   // if output is valid texture it's buffer will be re-used, otherwise it will be re-created

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


		public Mat boxMat(int partID)
        {
			//5 and 6 for eyes/partIDs
			if (processor.Faces.Count >= 1)
			{
				DetectedFace face = processor.Faces[0];

				int lLeftX = face.Elements[partID].Marks[0].X;
				int lRightX = face.Elements[partID].Marks[0].X;
				int lLowY = face.Elements[partID].Marks[0].Y;
				int lHighY = face.Elements[partID].Marks[0].Y;

				foreach (Point mark in face.Elements[partID].Marks)
				{

					if (mark.X < lLeftX)
					{
						lLeftX = mark.X;
					}

					if (mark.Y < lLowY)
					{
						lLowY = mark.Y;
					}

					if (mark.X > lRightX)
					{
						lRightX = mark.X;
					}

					if (mark.Y > lHighY)
					{
						lHighY = mark.Y;
					}

				}

				int eyeWidth = lRightX - lLeftX;
				int eyeHeight = lHighY - lLowY;

				OpenCvSharp.Rect eyeRect = new OpenCvSharp.Rect(lLeftX, lLowY, eyeWidth, eyeHeight);
				Mat refMat = new Mat(processor.Image, eyeRect);

				Mat croppedMat = new Mat();
				refMat.CopyTo(croppedMat);

				return croppedMat;
			}
			else
			{
				return null;

			}
		}


		public Tuple<float, float> FaceDetected()
		{
			if (processor.Faces.Count >= 1){
				DetectedFace face = processor.Faces[0];
				//Elements
				//5 - left eye
				//6 - right eye

				//Marks
				//2, 4 -- 1, 5 Opposite points

				//Test points and eyes vvvvv
				//print(face.Elements[rightOrLeftEye].Marks[pointMarkerTop] - face.Elements[rightOrLeftEye].Marks[pointMarkerBottom]);


				//left
				float rightEyeClosedPoint1 = face.Elements[5].Marks[4].Y - face.Elements[5].Marks[2].Y;
				float rightEyeClosedPoint2 = face.Elements[5].Marks[5].Y - face.Elements[5].Marks[1].Y;
				//right eye
				float leftEyeClosedPoint1 = face.Elements[6].Marks[4].Y - face.Elements[6].Marks[2].Y;
				float leftEyeClosedPoint2 = face.Elements[6].Marks[5].Y - face.Elements[6].Marks[1].Y;



				leftEyeAverage = (leftEyeClosedPoint1 + leftEyeClosedPoint2) / 2;
				rightEyeAverage = (rightEyeClosedPoint1 + rightEyeClosedPoint2) / 2;

				//print("left eye: " + leftEyeAverage);
				//print("right eye: " + rightEyeAverage);
				//print(face.Elements[5].Region);




				return Tuple.Create(leftEyeAverage, rightEyeAverage);
			}
            else
            {
				return Tuple.Create(0f, 0f);

			}


		}

       


        public Tuple<float, float> EyeBoxCoords()
		{
			if (processor.Faces.Count >= 1)
			{
				DetectedFace face = processor.Faces[0];

				//left
				Vector2 LUpLeftPoint = new Vector2(face.Elements[5].Marks[0].X, face.Elements[5].Marks[0].Y + face.Elements[5].Marks[1].Y);
				Vector2 LDownRightPoint = new Vector2(face.Elements[5].Marks[3].X, face.Elements[5].Marks[3].Y - face.Elements[5].Marks[4].Y);

				//right
				Vector2 RUpLeftPoint = new Vector2(face.Elements[6].Marks[0].X, face.Elements[6].Marks[0].Y + face.Elements[6].Marks[1].Y);
				Vector2 RDownRightPoint = new Vector2(face.Elements[6].Marks[3].X, face.Elements[6].Marks[3].Y - face.Elements[6].Marks[4].Y);




				return Tuple.Create(leftEyeAverage, rightEyeAverage);
			}
			else
			{
				return Tuple.Create(0f, 0f);

			}


		}




	}


}