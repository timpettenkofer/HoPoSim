using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class MeasureBehavior : MonoBehaviour
{
	public float defaultLineWidthMultiplier = 0.03f;
	public float lineWidthMultiplier = 0.03f;
	public float offset = 0.001f;

	private GameObject front;
	private GameObject back;

	public bool IsVisible(Side side)
	{
		if (side.HasFlag(Side.FRONT))
		{
			if (front == null || !front.activeInHierarchy)
				return false;
		}
		if (side.HasFlag(Side.BACK))
		{
			if (back == null || !back.activeInHierarchy)
				return false;
		}
		return true;
	}

	public void ForceBuild(Side side, float lineThicknessMultiplier)
	{
		Destroy(side);
		Show(side, lineThicknessMultiplier);
	}

	public void Destroy(Side side)
	{
		if (side.HasFlag(Side.FRONT))
		{
			Destroy(front);
			front = null;
		}
		if (side.HasFlag(Side.BACK))
		{
			Destroy(back);
			back = null;
		}
	}

	public void Show(Side side, float lineThicknessMultiplier = -1, bool verbose = false)
	{
		lineWidthMultiplier = lineThicknessMultiplier != -1 ? lineThicknessMultiplier : defaultLineWidthMultiplier;

		if (side.HasFlag(Side.FRONT))
		{
			if (front != null)
				front.SetActive(true);
			else
				front = Create(Side.FRONT, verbose);
		}
		if (side.HasFlag(Side.BACK))
		{
			if (back != null)
				back.SetActive(true);
			else
				back = Create(Side.BACK, verbose);
		}
	}

	public void Hide(Side side)
	{
		if (side.HasFlag(Side.FRONT) && front != null)
		{
			front.SetActive(false);
		}
		if (side.HasFlag(Side.BACK) && back != null)
		{
			back.SetActive(false);
		}
	}

	public abstract GameObject Create(Side side, bool verbose = false);

	protected static LineRenderer CreateLineRenderer(Vector3[] points, GameObject parent, float lineWidthMultiplier, Material material)
	{
		var go = new GameObject("line");
		go.transform.parent = parent.transform;

		LineRenderer lineRenderer = go.AddComponent<LineRenderer>();

		lineRenderer.useWorldSpace = true;
		lineRenderer.widthMultiplier = lineWidthMultiplier;
		lineRenderer.positionCount = points.Count();
		lineRenderer.SetPositions(points);
		lineRenderer.startColor = lineRenderer.endColor = Color.blue;
		lineRenderer.generateLightingData = false;
		lineRenderer.loop = true;
		lineRenderer.receiveShadows = false;

		lineRenderer.material = material;
		return lineRenderer;
	}

	static protected List<Vector2> CollectVertices(Side side, int pruningFactor)
	{
		var points = BoundingBox.CollectSectionPoints(side, pruningFactor);
		var vertices = points.
			Select((p, i) => new Vector2(p.x, p.y)).
			ToList();
		return vertices;
	}
}

