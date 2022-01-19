using UnityEngine;

public class PhysicMaterialController : MonoBehaviour
{
	// Expose properties in the inspector for easy adjustment.
	public float dynamicFriction;
	public float staticFriction;

	void Start()
	{
		foreach(var collider in GetComponentsInChildren<Collider>())
		{
			collider.material.dynamicFriction = dynamicFriction;
			collider.material.staticFriction = staticFriction;
			collider.material.frictionCombine = PhysicMaterialCombine.Maximum;
		}
	}
}