using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PolterManager
{
	public static int PolterunterlageLayer = 8;
	public static int PolterLayer = 9;
	public static int PolterToolsLayer = 10;

	public static string PolterunterlageTag = "Polterunterlage";
	public static string PolterTag = "Polter";
	public static string PolterParent = "Trunks";

	public static IEnumerable<GameObject> GetPolterTrunks(string tag)
	{
		var parent = GameObject.Find(PolterParent);
		return parent
			.GetComponentsInChildren<TrunkComponent>(true)
			.Select(tc => tc.gameObject)
			.Where(go => go.tag == tag);
	}

	public static float GetPolterAverageDepth()
	{
		var tcs = PolterManager.GetPolterTrunkComponents<TrunkComponent>(PolterManager.PolterTag);
		return tcs.Any()? tcs.Average(t => t.trunkParameters.Length) : 0f;
	}

	public static float GetPolterCustomOrAverageDepth()
	{
		var depth = ConfigurationHelper.SimulationData.Poltermaße.Poltertiefe;
		return depth.HasValue ? depth.Value : GetPolterAverageDepth();
	}

	public static float GetPolterMaxDepth()
	{
		var tcs = PolterManager.GetPolterTrunkComponents<TrunkComponent>(PolterManager.PolterTag);
		return tcs.Any() ? tcs.Max(t => t.trunkParameters.Length) : 0f;
	}

	public static void SetPolterTrunks(IEnumerable<GameObject> trunks)
	{
		foreach (var t in trunks)
		{
			t.gameObject.tag = PolterTag;
			SetupDefaultRigidBody(t, RigidbodyConstraints.None, 0f, 0f);
			SetSelfAndChildrenLayer(t.gameObject, PolterLayer);
		}
	}

	public static void SetupDefaultRigidBody(GameObject t, RigidbodyConstraints constraints, float drag = 0, float angularDrag = 0)
	{
		var rb = t.gameObject.GetComponent<Rigidbody>();
		rb.isKinematic = false;
		rb.constraints = constraints;
		rb.angularDrag = angularDrag;
		rb.drag = drag;
		rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
	}

	public static void SetKinematic(GameObject t)
	{
		var rb = t.gameObject.GetComponent<Rigidbody>();
		rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
		rb.isKinematic = true;
	}

	public static void SetPolterunterlageTrunks(IEnumerable<GameObject> trunks)
	{
		foreach (var t in trunks)
		{
			t.gameObject.tag = PolterunterlageTag;
			SetupDefaultRigidBody(t, RigidbodyConstraints.None, 3.5f, 0);
			SetSelfAndChildrenLayer(t.gameObject, PolterunterlageLayer);
		}
	}


	public static void SetPolterTools(GameObject go)
	{
		SetSelfAndChildrenLayer(go.gameObject, PolterToolsLayer);
	}

	private static void SetSelfAndChildrenLayer(GameObject go, int layer)
	{
		go.layer = layer;
		foreach (var transfom in go.GetComponentsInChildren<Transform>(true))
			transfom.gameObject.layer = layer;
	}

	public static T[] GetPolterTrunkComponents<T>(string tag)
	{
		return GetPolterTrunks(tag)
			.Select(t => t.GetComponent<T>())
			.ToArray();
	}

	public static void IgnoreCollision(int layer1, int layer2)
	{
		Physics.IgnoreLayerCollision(layer1, layer2);
	}
}

