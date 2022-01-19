using UnityEngine;

public class ScreenshotHandler : MonoBehaviour
{
	private static ScreenshotHandler instance;

	private Camera cam;
	private bool takeScreenshotOnNextFrame;
	private string outfile;

	private void Awake()
	{
		instance = this;
	}

	private void OnPostRender()
	{
		if (takeScreenshotOnNextFrame)
		{
			takeScreenshotOnNextFrame = false;
			var renderTexture = cam.targetTexture;
			var renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
			var rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
			renderResult.ReadPixels(rect, 0, 0);
			var byteArray = renderResult.EncodeToPNG();
			System.IO.File.WriteAllBytes(outfile, byteArray);
			ConfigurationHelper.Callback.Log($"Saved image to {outfile}");

			RenderTexture.ReleaseTemporary(renderTexture);
			cam.targetTexture = null;
			cam.enabled = false;
		}
	}

	private void EnsureCamera()
	{
		if (cam == null)
		{
			cam = gameObject.AddComponent<Camera>();
			cam.name = "Screenshot";
		}
	}

	private void TakeScreenshotInternal(string filepath, int width, int height)
	{
		outfile = filepath;
		EnsureCamera();
		cam.CopyFrom(Camera.main);
		takeScreenshotOnNextFrame = true;
		cam.targetTexture = RenderTexture.GetTemporary(width, height, 16);
		// Render the camera's view.
		cam.Render();
	}

	public static void TakeScreenshot(string filepath, int width, int height)
	{
		instance.TakeScreenshotInternal(filepath, width, height);
	}
}
