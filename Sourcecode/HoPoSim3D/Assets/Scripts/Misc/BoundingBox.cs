using HoPoSim.IPC.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class BoundingBox
{
	public static Bounds GetBounds(IEnumerable<Vector3> vertices)
	{
		if (!vertices.Any())
			return new Bounds();
		Bounds bounds = new Bounds(vertices.First(), Vector3.zero);
		foreach (var vertex in vertices)
			bounds.Encapsulate(vertex);
		return bounds;
	}

	public static Bounds GetRendererBounds(IEnumerable<GameObject> gos)
	{
		var renderers = gos.SelectMany(go => go.GetComponentsInChildren<Renderer>());
		return GetBounds(renderers.Select(c => c.bounds));
	}

	public static Bounds GetRendererBounds(GameObject obj)
	{
		var renderers = obj.GetComponentsInChildren<Renderer>();
		return GetBounds(renderers.Select(r => r.bounds));
	}

	public static Bounds GetColliderBounds(IEnumerable<GameObject> gos)
	{
		var colliders = gos.SelectMany(go => go.GetComponentsInChildren<Collider>());
		return GetBounds(colliders.Select(c => c.bounds));
	}

	public static Bounds GetColliderBounds(GameObject obj)
	{
		var colliders = obj.GetComponentsInChildren<Collider>();
		return GetBounds(colliders.Select(r => r.bounds));
	}

	public static Bounds GetAxisAlignedBounds(IEnumerable<GameObject> gos)
	{
		var bounds = gos.Select(go => GetAxisAlignedBounds(go));
		return GetBounds(bounds);
	}

	public static Bounds GetAxisAlignedBounds(GameObject obj)
	{
		var mesh = obj.GetComponent<MeshFilter>();
		if (mesh == null)
			return new Bounds();
		var trans = obj.transform;
		Vector3[] vertices = mesh.sharedMesh.vertices;
		if (!vertices.Any())
			return new Bounds();
		Vector3 min, max;
		min = max = trans.TransformPoint(vertices[0]);
		for (int i = 1; i < vertices.Length; i++)
		{
			var v = trans.TransformPoint(vertices[i]);
			for (int n = 0; n < 3; n++)
			{
				if (v[n] > max[n])
					max[n] = v[n];
				if (v[n] < min[n])
					min[n] = v[n];
			}
		}
		Bounds bounds = new Bounds();
		bounds.SetMinMax(min, max);
		return bounds;
	}

	private static Bounds GetBounds(IEnumerable<Bounds> boxes)
	{
		if (!boxes.Any())
			return new Bounds();
		Bounds bounds = boxes.First();
		foreach (var box in boxes)
			bounds.Encapsulate(box);
		return bounds;
	}

	public static Bounds GetPolterBounds(Side side)
	{
		var points = CollectSectionPoints(side, 1);
		return GetBounds(points);
	}

	public static Bounds GetPolterBounds(PolterConfiguration configuration)
	{
		var length = configuration.MinimumPolterlänge;
		var depth = configuration.Polterbreite;
		var height = 2;

		Bounds bounds = new Bounds();
		var min = new Vector3(0, 0, 0);
		var max = new Vector3(length, height, depth);
		bounds.SetMinMax(min, max);
		return bounds;
	}

	public static IEnumerable<Vector3> CollectSectionPoints(Side side, int pruningFactor)
	{
		var points = CollectAllPoints(pruningFactor);
		Func<Vector3, bool> filter = (side == Side.FRONT) ?
			(Func<Vector3, bool>)(p => p.z < 0) :
			(Func<Vector3, bool>)(p => p.z > 0);
		return points.Where(filter);
	}

	static List<Vector3> CollectAllPoints(int pruningFactor)
	{
		var tcs = PolterManager.GetPolterTrunkComponents<TrunkComponent>(PolterManager.PolterTag);
		List<Vector3> points = new List<Vector3>();

		foreach (var tc in tcs)
		{
			var topOutline = tc.trunk.topOutline.Where((p, i) => i % pruningFactor == 0);
			var bottomOutline = tc.trunk.bottomOutline.Where((p, i) => i % pruningFactor == 0);
			points.AddRange(topOutline.Select(p => tc.gameObject.transform.TransformPoint(p)));
			points.AddRange(bottomOutline.Select(p => tc.gameObject.transform.TransformPoint(p)));
		}
		return points;
	}

	//private static void ShowBB(GameObject trunk, Bounds bounds)
	//{
	//	CreatePoint(new Vector3(bounds.min.x, bounds.min.y, bounds.min.z), trunk.name + "_1");
	//	CreatePoint(new Vector3(bounds.min.x, bounds.min.y, bounds.max.z), trunk.name + "_2");
	//	CreatePoint(new Vector3(bounds.min.x, bounds.max.y, bounds.min.z), trunk.name + "_3");
	//	CreatePoint(new Vector3(bounds.min.x, bounds.max.y, bounds.max.z), trunk.name + "_4");
	//	CreatePoint(new Vector3(bounds.max.x, bounds.min.y, bounds.min.z), trunk.name + "_5");
	//	CreatePoint(new Vector3(bounds.max.x, bounds.max.y, bounds.min.z), trunk.name + "_6");
	//	CreatePoint(new Vector3(bounds.max.x, bounds.min.y, bounds.max.z), trunk.name + "_7");
	//	CreatePoint(new Vector3(bounds.max.x, bounds.max.y, bounds.max.z), trunk.name + "_8");
	//}

	//private static void CreatePoint(Vector3 pos, string name)
	//{
	//	var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
	//	go.name = name;
	//	go.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
	//	go.transform.position = pos;
	//}
}
