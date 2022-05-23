namespace OpenCvSharp.Demo
{
	using System;
	using UnityEngine;
	using System.Collections.Generic;
	using UnityEngine.UI;
	using OpenCvSharp;
	using OpenCvSharp.Tracking;
	using System.Threading;

	public class FaceDetectorScene : WebCamera
	{
		public float leftEyeAverage, rightEyeAverage;

		public TextAsset faces;
		public TextAsset eyes;
		public TextAsset shapes;

		private FaceProcessorLive<Mat> processor;

		Mat mat;
		public Mat croppedMat;

		// downscaling const
		const float downScale = 0.15f;
		const float minimumAreaDiagonal = 25.0f;



		public bool readyToTrack;

		// tracker
		Size frameSize = Size.Zero;
		Tracker tracker = null;


		/// <summary>
		/// Default initializer for MonoBehavior sub-classes
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			base.forceFrontalCamera = true; // we work with frontal cams here, let's force it for macOS s MacBook doesn't state frontal cam correctly

			byte[] shapeDat = shapes.bytes;

			processor = new FaceProcessorLive<Mat>();
			processor.Initialize(faces.text, eyes.text, shapes.bytes);

			// data stabilizer - affects face rects, face landmarks etc.
			processor.DataStabilizer.Enabled = false;        // enable stabilizer
			processor.DataStabilizer.Threshold = 1;       // threshold value in pixels
			processor.DataStabilizer.SamplesCount = 2;      // how many samples do we need to compute stable data

			// performance data - some tricks to make it work faster
			//processor.Performance.Downscale = 256;          // processed image is pre-scaled down to N px by long side
			processor.Performance.SkipRate = 0;             // we actually process only each Nth frame (and every frame for skipRate = 0)





		}


		/// <summary>
		/// Per-frame video capture processor
		/// </summary>
		protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
		{
			mat = Unity.TextureToMat(input);

			int firstFaceX = (int)(mat.Width * 0.38f);
			int firstFaceY = (int)(mat.Height * 0.2f);
			int lengthFaceX = (int)(mat.Width * 0.62f) - firstFaceX;
			int lengthFaceY = (int)(mat.Height * 0.70f) - firstFaceY;


			OpenCvSharp.Rect areaRect = new OpenCvSharp.Rect(firstFaceX, firstFaceY, lengthFaceX, lengthFaceY);

			areaRect = areaRect * downScale;

			Cv2.Rectangle((InputOutputArray)mat, areaRect * (1.0 / downScale), Scalar.FromRgb(255, 255, 0), 3);

			



			// processor.Image now holds data we'd like to visualize
			Mat downscaled = mat.Resize(Size.Zero, downScale, downScale);

			//Cv2.Rectangle((InputOutputArray)mat, areaRect, Scalar.FromRgb(255, 255, 0), 3);
			Rect2d obj = Rect2d.Empty;

			// If not dragged - show the tracking data
			if (readyToTrack)
			{

				// we have to tracker - let's initialize one
				if (null == tracker)
				{
					obj = new Rect2d(areaRect.X, areaRect.Y, areaRect.Width, areaRect.Height);

					// initial tracker with current image and the given rect, one can play with tracker types here
					tracker = Tracker.Create(TrackerTypes.KCF);
					tracker.Init(downscaled, obj);

					frameSize = downscaled.Size();
				}
				// if we already have an active tracker - just to to update with the new frame and check whether it still tracks object
				else
				{
					if (!tracker.Update(downscaled, ref obj))
						obj = Rect2d.Empty;
				}

				// save tracked object location
				if (0 != obj.Width && 0 != obj.Height)
					areaRect = new OpenCvSharp.Rect((int)obj.X, (int)obj.Y, (int)obj.Width, (int)obj.Height);
			}
			else
			{
				DropTracking();
			}

			// render rect we've tracker or one is being drawn by the user
			if (null != tracker && obj.Width != 0)
				Cv2.Rectangle((InputOutputArray)mat, areaRect * (1.0 / downScale), Scalar.FromRgb(255, 255, 0), 3);
			//Cv2.Rectangle((InputOutputArray)image, areaRect * (1.0 / downScale), isDragging? Scalar.Red : Scalar.LightGreen);


			croppedMat = new Mat(mat, areaRect * (1.0 / downScale));

			processor.Performance.Downscale = (int)(croppedMat.Height * downScale);

			Thread t = new Thread(processFace);
			t.Start();

			processor.MarkDetected();

			// result, passing output texture as parameter allows to re-use it's buffer
			// should output texture be null a new texture will be created
			output = Unity.MatToTexture(mat, output);
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

		void processFace()
        {
			// detect everything we're interested in
			processor.ProcessTexture(croppedMat, true);


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
				float rightEyeClosedPoint1 = 0; //face.Elements[5].Marks[4].Y - face.Elements[5].Marks[2].Y;
				float rightEyeClosedPoint2 = 0; //face.Elements[5].Marks[5].Y - face.Elements[5].Marks[1].Y;
												//right eye
				float leftEyeClosedPoint1 = 0; //face.Elements[6].Marks[4].Y - face.Elements[6].Marks[2].Y;
				float leftEyeClosedPoint2 = 0; //face.Elements[6].Marks[5].Y - face.Elements[6].Marks[1].Y;



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

		protected void DropTracking()
		{
			if (null != tracker)
			{
				tracker.Dispose();
				tracker = null;

				//startPoint = endPoint = Vector2.zero;
			}
		}



	}


}