namespace OpenCvSharp.Demo
{
	using UnityEngine;
	using UnityEngine.EventSystems;

	using System;

	using OpenCvSharp;
	using OpenCvSharp.Tracking;

	/// <summary>
	/// Object tracking handler
	/// </summary>
	public class TrackingScript : WebCamera
	{
		// downscaling const
		const float downScale = 1f;
		const float minimumAreaDiagonal = 25.0f;



		public bool readyToTrack;

		// tracker
		Size frameSize = Size.Zero;
		Tracker tracker = null;

		/// <summary>
		/// Initialization
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			forceFrontalCamera = true;
		}

		/// <summary>
		/// Converts point from screen space into the image space
		/// </summary>
		/// <param name="coord"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		Vector2 ConvertToImageSpace(Vector2 coord, Size size)
		{
			var ri = GetComponent<UnityEngine.UI.RawImage>();

			Vector2 output = new Vector2();
			RectTransformUtility.ScreenPointToLocalPointInRectangle(ri.rectTransform, coord, null, out output);

			// pivot is in the center of the rectTransform, we need { 0, 0 } origin
			output.x += size.Width / 2;
			output.y += size.Height / 2;

			// now our image might have various transformations of it's own
			if (!TextureParameters.FlipVertically)
				output.y = size.Height - output.y;
			
			// downscaling
			output.x *= downScale;
			output.y *= downScale;

			return output;
		}

		/// <summary>
		/// Our main function to process the tracking:
		/// 1. If there is no active dragging and no area - it does nothing useful, just renders the image
		/// 2. If there is active dragging - it renders image with dragging rectangle over it (red color)
		/// 3. if there is no dragging, but there is tracker and it has result - it draws image with tracked object rect over it (green color)
		/// </summary>
		/// <param name="input"></param>
		/// <param name="output"></param>
		/// <returns></returns>
		protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
		{
			Mat image = Unity.TextureToMat(input, TextureParameters);
			Mat downscaled = image;//.Resize(Size.Zero, downScale, downScale);

			int firstFaceX = (int)((image.Width * 0.38f) * downScale);
			int firstFaceY = (int)((image.Height * 0.2f) * downScale);
			int lengthFaceX = (int)(((image.Width * 0.62f) - firstFaceX) * downScale);
			int lengthFaceY = (int)(((image.Height * 0.60f) - firstFaceY) * downScale);

			OpenCvSharp.Rect areaRect = new OpenCvSharp.Rect(firstFaceX, firstFaceY, lengthFaceX, lengthFaceY);
			Cv2.Rectangle((InputOutputArray)image, areaRect, Scalar.FromRgb(255, 255, 0), 3);
			Rect2d obj = Rect2d.Empty;

			// If not dragged - show the tracking data
			if (readyToTrack)
			{
				// drop tracker if the frame's size has changed, this one is necessary as tracker doesn't hold it well
				if (frameSize.Height != 0 && frameSize.Width != 0 && downscaled.Size() != frameSize)
					DropTracking();

				// we have to tracker - let's initialize one
				if (null == tracker)
				{
					obj = new Rect2d(areaRect.X, areaRect.Y, areaRect.Width, areaRect.Height);

					// initial tracker with current image and the given rect, one can play with tracker types here
					tracker = Tracker.Create(TrackerTypes.MedianFlow);
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
				Cv2.Rectangle((InputOutputArray)image, areaRect, Scalar.FromRgb(255, 255, 0), 3);
			//Cv2.Rectangle((InputOutputArray)image, areaRect * (1.0 / downScale), isDragging? Scalar.Red : Scalar.LightGreen);

			// result, passing output texture as parameter allows to re-use it's buffer
			// should output texture be null a new texture will be created
			output = Unity.MatToTexture(image, output);
			return true;
		}

		/// <summary>
		/// Frees object tracker
		/// </summary>
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