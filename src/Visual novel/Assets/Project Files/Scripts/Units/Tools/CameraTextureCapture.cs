#region

using UnityEngine;

#endregion

namespace Units.Tools
{
	public static class CameraTextureCapture
	{
		private const int IGNORE_LAYER = 31;

		public static Texture2D CaptureCameraView(params Canvas[] canvases)
		{
			const int captureWidth = 1920;
			const int captureHeight = 1080;
			var captureCamera = new GameObject("CaptureCamera").AddComponent<Camera>();

			foreach (var canvas in canvases)
			{
				canvas.renderMode = RenderMode.ScreenSpaceCamera;
				canvas.worldCamera = captureCamera;
			}

			var renderTexture = new RenderTexture(captureWidth, captureHeight, 24);
			captureCamera.targetTexture = renderTexture;
			captureCamera.Render();

			foreach (var canvas in canvases)
			{
				canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			}

			var capturedTexture = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);
			RenderTexture.active = renderTexture;
			capturedTexture.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
			capturedTexture.Apply();

			RenderTexture.active = null;
			captureCamera.targetTexture = null;
			Object.Destroy(captureCamera.gameObject);
			Object.Destroy(renderTexture);

			return capturedTexture;
		}
	}
}