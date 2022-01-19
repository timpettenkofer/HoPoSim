using ConcaveHull;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

public class ConcaveHullOutline : MeasureBehavior
{
	public double concavity = 0f;
	public int scaleFactor = 3;

	public float ComputeArea(Side side, out Vector2[] vertices)
	{
		int pruningFactor = ConfigurationHelper.SimulationSettings.Quality;
		ConfigurationHelper.Callback.Log($"Computing concave hull area for side {side} (pruning factor: {pruningFactor})...");
		vertices = ComputeHullPolyline(side, pruningFactor);
		return Area.ComputeArea(vertices.ToList()); 
	}

	public override GameObject Create(Side side, bool verbose = false)
	{
		try
		{
			var hull = new GameObject("Hull");
			hull.transform.SetParent(gameObject.transform);
			int pruningFactor = ConfigurationHelper.SimulationSettings.Quality;
			ConfigurationHelper.Callback.Log($"Computing hull for side {side} (pruning factor: {pruningFactor})...");
			var vertices = ComputeHullPolyline(side, pruningFactor);
			var points = PolterTransform.Transform3dAverageDepth(vertices, side, offset);
			Material material = (Material)Resources.Load("Materials/Hull");
			CreateLineRenderer(points, hull, lineWidthMultiplier, material);
			return hull;
		}
		catch(Exception e)
		{
			ConfigurationHelper.Callback.Log($"Error while computing hull: {e.Message}");
			return null;
		}
	}

	//private static void CreateLineRenderers(List<Line> lines, GameObject parent, float z)
	//{
	//	foreach(var line in lines)
	//	{
	//		var n0 = line.nodes[0].ToVector2();
	//		var n1 = line.nodes[1].ToVector2();
	//		var go = new GameObject();
	//		go.transform.parent = parent.transform;
	//		CreateLineRenderer(new Vector3[] { new Vector3(n0.x, n0.y, z), new Vector3(n1.x, n1.y, z) }, go);
	//	}
	//}

	public Vector2[] ComputeHullPolyline(Side side, int pruningFactor)
	{
		try
		{
			var lines = ComputeConcaveHull(side, pruningFactor);
			var points = ExtractPolyline(lines);
			return points;
		}
		catch(Exception e)
		{
			ConfigurationHelper.Callback.Log($"Error while computing hull polyline: {e.GetBaseException()}");
			return new Vector2[0];
		}
	}

	Vector2[] ExtractPolyline(List<Line> lines)
	{
		if (!lines.Any())
			return new Vector2[0];

		var polyline = new List<Vector2>();

		var currentLine = lines[0];
		lines.RemoveAt(0);

		polyline.Add(currentLine.nodes[0].ToVector2());
		var nextNodeIndex = 1;

		while (currentLine != null)
		{
			var node = currentLine.nodes[nextNodeIndex];
			polyline.Add(node.ToVector2());

			currentLine = lines.FirstOrDefault(l => l.nodes[0].id == node.id || l.nodes[1].id == node.id);		
			if (currentLine != null)
			{
				nextNodeIndex = currentLine.nodes[0].id == node.id ? 1 : 0;
				lines.Remove(currentLine);
			}
		}

		return polyline.ToArray();
	}


	List<Line> ComputeConcaveHull(Side side, int pruningFactor)
	{
		var nodes = CollectNodes(side, pruningFactor);
		if (!nodes.Any())
			return new List<Line>();
		var hull = new Hull();
		hull.setConvexHull(nodes);
		hull.setConcaveHull(concavity, scaleFactor);
		return hull.hull_concave_edges;
	}

	static List<Node> CollectNodes(Side side, int pruningFactor)
	{
		var points = BoundingBox.CollectSectionPoints(side, pruningFactor);

		var nodes = points.
			Select((p, i) => new Node(p.x, p.y, i)).
			ToList();
		return nodes;
	}
}
