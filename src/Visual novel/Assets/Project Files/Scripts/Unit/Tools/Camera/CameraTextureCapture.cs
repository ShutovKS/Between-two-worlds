#region

using UnityEngine;

#endregion

namespace Tools.Camera
{
    public static class CameraTextureCapture
    {
        public static Texture2D CaptureCameraView(params Canvas[] canvases)
        {
            const int CAPTURE_WIDTH = 1920;
            const int CAPTURE_HEIGHT = 1080;

            var captureCamera = new GameObject("CaptureCamera").AddComponent<UnityEngine.Camera>();
            captureCamera.targetDisplay = 2;

            foreach (var canvas in canvases)
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = captureCamera;
                canvas.targetDisplay = 2;
            }

            var renderTexture = new RenderTexture(CAPTURE_WIDTH, CAPTURE_HEIGHT, 24);
            captureCamera.targetTexture = renderTexture;
            captureCamera.Render();

            foreach (var canvas in canvases)
            {
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.worldCamera = null;
                canvas.targetDisplay = 1;
            }

            var capturedTexture = new Texture2D(CAPTURE_WIDTH, CAPTURE_HEIGHT, TextureFormat.RGB24, false);
            RenderTexture.active = renderTexture;
            capturedTexture.ReadPixels(new Rect(0, 0, CAPTURE_WIDTH, CAPTURE_HEIGHT), 0, 0);
            capturedTexture.Apply();

            RenderTexture.active = null;
            captureCamera.targetTexture = null;
            Object.Destroy(captureCamera.gameObject);
            Object.Destroy(renderTexture);

            return capturedTexture;
        }
    }
}