using UnityEngine;
using System;
using System.Linq;
using Utils;
using System.Collections.Generic;

public class Fotooptik : MeasureBehavior
{
	public int numVerticesPerMeter;

	public void Awake()
	{
		numVerticesPerMeter = 3;
	}

	public float ComputeArea(Side side, Vector2[] hull, out int numVertices)
	{
		int quality = ConfigurationHelper.SimulationSettings.FotooptikQuality;
		ConfigurationHelper.Callback.Log($"Computing fotooptik hull area for side {side} (average number of support points per lfm: {quality})...");
		var vertices = ComputeFotooptikHull(side, hull, quality);
		numVertices = vertices.Count();
		return Area.ComputeArea(vertices.ToList());
	}

	public override GameObject Create(Side side, bool verbose = false)
	{
		try
		{
			var fotooptik = new GameObject("Fotooptik");
			fotooptik.transform.SetParent(gameObject.transform);

			int pruningFactor = ConfigurationHelper.SimulationSettings.Quality;
			int quality = ConfigurationHelper.SimulationSettings.FotooptikQuality;
			ConfigurationHelper.Callback.Log($"Computing fotooptik hull for side {side} (pruning factor: {pruningFactor}, average number of support points per lfm: {quality})...");
			Vector2[] hull = GetConcaveHullOutline(side, pruningFactor);
			var vertices = ComputeFotooptikHull(side, hull, quality);
			var points = PolterTransform.Transform3dAverageDepth(vertices, side, offset);
			Material material = (Material)Resources.Load("Materials/Fotooptik");
			CreateLineRenderer(points, fotooptik, lineWidthMultiplier, material);
			return fotooptik;
		}
		catch (Exception e)
		{
			ConfigurationHelper.Callback.Log($"Error while computing convex hull: {e.Message}");
			return null;
		}
	}

	private Vector2[] GetConcaveHullOutline(Side side, int pruningFactor)
	{
		var hc = gameObject.GetComponent<ConcaveHullOutline>();
		var hull = hc.ComputeHullPolyline(side, pruningFactor);
		return hull;
	}

	public Vector2[] ComputeFotooptikHull(Side side, Vector2[] concaveHull, int averageNumberOfVerticesPerMeter)
	{
		try
		{
			var points = concaveHull.ToList();
			int count = points.Count();
			// ensure that first and lat points are distincts
			if (count > 1 && points[0] == points[count - 1])
				points.RemoveAt(0);

			if (!points.Any())
				return new Vector2[0];

			return SimplifyHull(points, averageNumberOfVerticesPerMeter);
		}
		catch (Exception e)
		{
			ConfigurationHelper.Callback.Log($"Error while computing convex hull polyline: {e.GetBaseException()}");
			return new Vector2[0];
		}
	}

	public static Vector2[] SimplifyHull(List<Vector2> hull, int averageNumberOfVerticesPerMeter)
	{
		var expectedNumVertices = GetPolylineLength(hull) * averageNumberOfVerticesPerMeter;
		var results = hull.ToList();
		var signedArea = Area.ComputeSignedArea(results);

		int indexToRemove; 
		while (results.Count > expectedNumVertices)
		{
			indexToRemove = GetSmallestConcaveVertexIndex(results, signedArea);
			if (indexToRemove == -1)
				return results.ToArray();
			results.RemoveAt(indexToRemove);
		}
		return results.ToArray();
	}

	private static float GetPolylineLength(List<Vector2> hull)
	{
		int count = hull.Count;
		if (count < 2)
			return 0;

		var length = Vector2.Distance(hull[0], hull[count - 1]);
		for(int i = 0; i < count - 1; ++i)
		{
			length += Vector2.Distance(hull[i], hull[i + 1]);
		}
		return length;
	}

	private static int GetSmallestConcaveVertexIndex(List<Vector2> results, float signedPolygonArea)
	{
		int numVertices = results.Count;
		float minSquaredArea = Single.MaxValue;
		int index = -1;

		for (int i = 0; i < numVertices; i++)
		{
			var area = Area.ComputeSignedArea(results[(i - 1 + numVertices) % numVertices], results[i % numVertices], results[(i + 1) % numVertices]);
			if (signedPolygonArea *  area < 0)
			{
				var squaredArea = area * area;
				if (squaredArea < minSquaredArea)
				{
					minSquaredArea = squaredArea;
					index = i;
				}
			}
		}
		return index;
	}
	
	/// <summary>Computes the convex hull of a polygon, in clockwise order in a Y-up 
	/// coordinate system (counterclockwise in a Y-down coordinate system).</summary>
	/// <remarks>Uses the Monotone Chain algorithm, a.k.a. Andrew's Algorithm.</remarks>
	public static IList<Vector2> ComputeConvexHull(List<Vector2> points)
	{
		points.Sort((a, b) =>
			a.x == b.x ? a.y.CompareTo(b.y) : (a.x > b.x ? 1 : -1));

		// Importantly, DList provides O(1) insertion at beginning and end
		List<Vector2> hull = new List<Vector2>();
		int L = 0, U = 0; // size of lower and upper hulls

		// Builds a hull such that the output polygon starts at the leftmost point.
		for (int i = points.Count - 1; i >= 0; i--)
		{
			Vector2 p = points[i], p1;

			// build lower hull (at end of output list)
			
			while (L >= 2 && Cross((p1 = hull[hull.Count - 1]) - hull[hull.Count - 2], p - p1) >= 0)
			{
				hull.RemoveAt(hull.Count - 1);
				L--;
			}
			hull.Add(p);
			L++;

			// build upper hull (at beginning of output list)
			while (U >= 2 && Cross((p1 = hull[0]) - hull[1], p - p1) <= 0)
			{
				hull.RemoveAt(0);
				U--;
			}
			if (U != 0) // share the point added above, except in the first iteration
				hull.Insert(0, p);
			U++;
			Debug.Assert(U + L == hull.Count + 1);
		}
		hull.RemoveAt(hull.Count - 1);
		return hull;
	}

	private static float Cross(Vector2 a, Vector2 b)
	{
		return (a.x * b.y - a.y * b.x);
	}
}
