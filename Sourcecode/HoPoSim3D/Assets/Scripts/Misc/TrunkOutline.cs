using System.Linq;
using UnityEngine;
using Utils;

public class TrunkOutline : MonoBehaviour
{
	private GameObject outline;
	public Color color = Color.black;
	public float offset = 0.01f;

	private LineRenderer bottom;
	private LineRenderer top;

	void Start() { Init(); }

	public void Init()
	{
		outline = new GameObject("Outline");
		outline.transform.SetParent(gameObject.transform, false);
		int pruningFactor = 1;
		top = CreateOutline(outline.transform, GetTopOutlineVertices(pruningFactor), offset);
		bottom = CreateOutline(outline.transform, GetBottomOutlineVertices(pruningFactor), -offset);
		SetVisible(false);
	}

	private Vector3[] GetTopOutlineVertices(int pruningFactor, bool toWorldCoordinates = false)
	{
		var tc = gameObject.GetComponent<TrunkComponent>();
		return tc.trunk.topOutline.
			Where((p, i) => i % pruningFactor == 0).
			Select(p => toWorldCoordinates ? tc.gameObject.transform.TransformPoint(p) : p).
			ToArray();
	}

	private Vector3[] GetBottomOutlineVertices(int pruningFactor, bool toWorldCoordinates = false)
	{
		var tc = gameObject.GetComponent<TrunkComponent>();
		return tc.trunk.bottomOutline.
			Where((p, i) => i % pruningFactor == 0).
			Select(p => toWorldCoordinates ? tc.gameObject.transform.TransformPoint(p) : p).
			ToArray();
	}

	public float ComputeArea(Side side)
	{
		int pruningFactor = ConfigurationHelper.SimulationSettings.Quality;
		var vertices = GetOutlineVerticesForSide(side, pruningFactor);
		var points  = vertices.
			Select(v => new Vector2(v.x, v.y)).
			ToList();
		return Area.ComputeArea(points);
	}

	private Vector3[] GetOutlineVerticesForSide(Side side, int pruningFactor)
	{
		var bottomOutline = GetBottomOutlineVertices(pruningFactor, true);
		var topOutline = GetTopOutlineVertices(pruningFactor, true);

		if (topOutline.Count() == 0)
			return new Vector3[0];

		switch (side)
		{
			case Side.FRONT:
				return topOutline[0].z < 0 ? topOutline : bottomOutline;
			case Side.BACK:
				return topOutline[0].z < 0 ? bottomOutline : topOutline;
			default:
				return new Vector3[0];
		}
	}

	private LineRenderer CreateOutline(Transform parent, Vector3[] points, float offsetY)
	{
		GameObject go = new GameObject("line");
		go.transform.SetParent(parent,false);
		go.transform.Translate(0, offsetY, 0);
		LineRenderer lineRenderer = go.AddComponent<LineRenderer>();
		lineRenderer.useWorldSpace = false;
		lineRenderer.widthMultiplier = 0.02f;
		lineRenderer.positionCount = points.Count();
		lineRenderer.SetPositions(points);
		lineRenderer.startColor = lineRenderer.endColor = color;
		lineRenderer.generateLightingData = false;
		lineRenderer.loop = true;
		lineRenderer.receiveShadows = false;
		lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		return lineRenderer;
	}

	public bool IsVisible()
	{
		return outline != null && outline.activeInHierarchy;
	}

	public void SetVisible(bool visible)
	{
		if (outline == null) Init();
		outline.SetActive(visible);
	}
}
