using UnityEngine;

public class TrunkPhysics : MonoBehaviour
{
	void Awake()
	{
		AddPhysics();
		var rigidbody = gameObject.GetComponent<Rigidbody>();
		var wood_density = ConfigurationHelper.SimulationData.WoodDensity * 0.1f; // kg / m3
		rigidbody.mass = GetVolume() * wood_density;
		rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		rigidbody.maxDepenetrationVelocity = 1e+5f;
	}

	void Start()
	{
		
	}

	public static void ConfigurePhysicWoodMaterial(float woodFriction)
	{
		var mat = (PhysicMaterial)Resources.Load("PhysicMaterials/Wood");
		mat.dynamicFriction = woodFriction;
		mat.staticFriction = woodFriction;
	}

	public void AddPhysics()
	{
		AddRigidBody();
		AddPhysicalMaterial("PhysicMaterials/Wood");
	}

	private void AddPhysicalMaterial(string name)
	{
		var colliders = gameObject.GetComponentsInChildren<Collider>();
		foreach(var collider in colliders)
			collider.material = (PhysicMaterial)Resources.Load(name);
	}

	//public void SetPolterunterlageConstraints()
	//{
	//	SetConstraints(RigidbodyConstraints.None);
	//	AddPhysicalMaterial("PhysicMaterials/LowFriction");
	//}

	//public void SetPolterConstraints()
	//{
	//	SetConstraints(RigidbodyConstraints.None);
	//	AddPhysicalMaterial("PhysicMaterials/Wood");
	//}

	//private void SetConstraints(RigidbodyConstraints constraints)
	//{
	//	var rigidbody = gameObject.GetComponent<Rigidbody>();
	//	rigidbody.constraints = constraints;
	//}


	private void AddRigidBody()
	{
		var rigidbody = gameObject.AddComponent<Rigidbody>();
		//rigidbody.SetDensity(1);
		rigidbody.useGravity = true;
	}

	private float GetVolume()
	{
		var properties = gameObject.GetComponent<MeshVolume>();
		return properties.FmmR;
	}
}
