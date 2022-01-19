using UnityEngine;
using UnityEngine.UI;

public class TrunkLabel : MonoBehaviour
{
	private GameObject bottomlabel;
	private GameObject toplabel;

	private float offset = 0.01f;

	public void Init()
	{
		var tc = gameObject.GetComponent<TrunkComponent>();
		var trunk = tc.trunk;
		var id = gameObject.name;
		var bottom = trunk.bottom;
		var top = trunk.top;
		var radiusBottom = trunk.bottom.radius;
		var radiusTop = trunk.top.radius;
		var transform = gameObject.transform;

		bottomlabel = CreateLabel(transform, id, new Vector3(bottom.position.x, bottom.position.y - offset, bottom.position.z), bottom.normal, radiusBottom);
		toplabel = CreateLabel(transform, id, new Vector3(top.position.x, top.position.y + offset, top.position.z), top.normal, radiusTop);
	}

	GameObject CreateLabel(Transform parent, string txt, Vector3 pos, Vector3 normal, float radius)
	{
		var canvas = CreateCanvas(parent, pos);
		canvas.SetActive(false);

		var q = ComputeRotation(parent.TransformVector(normal).normalized);
		var text = CreateText(canvas.transform, q, txt, 4, Color.green);

		ScaleLabel(canvas, text, radius);

		canvas.SetActive(true);
		return canvas;
	}

	private void ScaleLabel(GameObject canvas, GameObject text, float radius)
	{
		var c = text.GetComponent<Text>();

		var rectTransform = text.GetComponent<RectTransform>();
		Vector3[] corners = new Vector3[4];
		rectTransform.GetWorldCorners(corners);

		var trunk = gameObject.GetComponent<TrunkComponent>();
		var id = gameObject.name;

		float text_width = Mathf.Abs(corners[2].x - corners[0].x); 
		float text_height = Mathf.Abs(corners[2].y - corners[0].y);

		text_width = Mathf.Max(text_width, text_height) * 0.01f;

		float desired_size = radius * 0.5f;
		var factor = 0.4f * (desired_size / text_width);

		var canvasTransform = canvas.GetComponent<RectTransform>();
		canvasTransform.localScale = factor * canvasTransform.localScale;
	}


	Quaternion ComputeRotation(Vector3 normal)
	{
		var rot = Quaternion.FromToRotation(Vector3.back, normal);
		return Quaternion.Euler(rot.eulerAngles.x, rot.eulerAngles.y, 0);
	}

	GameObject CreateCanvas(Transform parent, Vector3 pos)
	{
		GameObject go = new GameObject("Info");
		go.transform.SetParent(parent);

		RectTransform trans = go.AddComponent<RectTransform>();
		trans.localScale = 0.1f * Vector3.one;
		trans.localPosition = new Vector3(pos.x, pos.y, pos.z);


		var scaler = go.AddComponent<CanvasScaler>();
		scaler.dynamicPixelsPerUnit = 30;
		scaler.referencePixelsPerUnit = 1;

		//var fitter = go.AddComponent<ContentSizeFitter>();
		//fitter.horizontalFit = ContentSizeFitter.FitMode.MinSize;
		//fitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;

		return go;
	}

	GameObject CreateText(Transform parent, Quaternion q, string text_str, int font_size, Color text_color)
	{
		GameObject t = new GameObject("Id");
		t.transform.SetParent(parent);

		RectTransform trans = t.AddComponent<RectTransform>();
		trans.localPosition = new Vector3(0, 0, 0);
		trans.transform.rotation = q;
		trans.localScale = Vector3.one;

		trans.anchorMin = Vector2.zero;
		trans.anchorMax = Vector2.one;
		trans.anchoredPosition = Vector2.zero;
		trans.sizeDelta = Vector2.zero;

		Text text = t.AddComponent<Text>();
		text.text = text_str;
		text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
		text.fontSize = font_size;
		text.color = text_color;
		text.verticalOverflow = VerticalWrapMode.Overflow;
		text.horizontalOverflow = HorizontalWrapMode.Overflow;
		text.alignment = TextAnchor.MiddleCenter;
		text.supportRichText = false;
		text.alignByGeometry = true;
		text.raycastTarget = false;

		//var fitter = t.AddComponent<ContentSizeFitter>();
		//fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
		//fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

		return t;
	}

	public bool IsVisible()
	{
		return bottomlabel != null;
	}

	public void SetVisible(bool visible)
	{
		Destroy(bottomlabel);
		Destroy(toplabel);
		bottomlabel = toplabel = null;

		if (visible)
			Init();
	}
}
