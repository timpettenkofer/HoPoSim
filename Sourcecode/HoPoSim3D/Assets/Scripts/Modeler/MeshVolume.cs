using MTrunk;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(TrunkComponent))]
public class MeshVolume : MonoBehaviour
{
	public float FmmR;
	public float FmoR;

	private float hr_radial_resolution = 16.0f;

	void Awake()
	{
		var trunk = gameObject.GetComponent<TrunkComponent>();
		var parameters = trunk.trunkParameters;
		
		var mr_parameters = GenerateParametersFrom(parameters, hr_radial_resolution, parameters.RadiusMultiplier);
		FmmR = ComputeVolume(mr_parameters);

		var or_parameters = GenerateParametersFrom(parameters, hr_radial_resolution, parameters.RadiusMultiplier - parameters.BarkThickness);
		FmoR = ComputeVolume(or_parameters);
	}

	private float ComputeVolume(TrunkParameters parameters)
	{
		var tc = gameObject.AddComponent<TrunkComponent>();
		var mesh = tc.GenerateMesh(parameters);
		DestroyImmediate(tc);
		return VolumeOfMesh(mesh);
	}

	private TrunkParameters GenerateParametersFrom(TrunkParameters parameters, float radialResolution, float radius)
	{
		var p = new TrunkParameters()
		{
			DisplacementSize = 0f, // set to 0 (instead of parameters.DisplacementSize) to avoid an overestimation of the volumes
			DisplacementStrength = 0f, // set to 0 (instead of parameters.DisplacementStrength) to avoid an overestimation of the volumes

			Length = parameters.Length,
			RadiusMultiplier = radius,
			Taper = parameters.Taper,
			BendingMultiplier = parameters.BendingMultiplier,
			BendingShape = parameters.BendingShape,
			Ovality = parameters.Ovality,
			RadialResolution = radialResolution,

			RootHeight = parameters.RootHeight,
			RootRadius = parameters.RootRadius,
			FlareNumber = parameters.FlareNumber,
			RootResolutionMultiplier = parameters.RootResolutionMultiplier,

			BranchNumber = 0
		};
		return p;
	}

	public static float VolumeOfMesh(Mesh mesh)
	{
		float volume = 0.0f;
		Vector3[] vertices = mesh.vertices;
		int[] triangles = mesh.triangles;

		// Computing the sum of the volumes of all the sub-polyhedrons
		for (int i = 0; i < mesh.triangles.Length; i += 3)
		{
			Vector3 p1 = vertices[triangles[i + 0]];
			Vector3 p2 = vertices[triangles[i + 1]];
			Vector3 p3 = vertices[triangles[i + 2]];
			volume += SignedVolumeOfTriangle(p1, p2, p3);
		}
		volume = volume / 6.0f;

		return volume > 0 ? volume : -volume;
	}

	public static float SignedVolumeOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
	{
		return Vector3.Dot(Vector3.Cross(p1, p2), p3);
	}
}
