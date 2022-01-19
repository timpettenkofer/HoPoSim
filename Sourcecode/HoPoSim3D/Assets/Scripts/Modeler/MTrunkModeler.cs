using Assets.Interfaces;
using HoPoSim.IPC.DAO;
using MTrunk;
using UnityEngine;

public class MTrunkModeler : MonoBehaviour, IModeler
{
	public GameObject CreateGeometry(Stamm stamm, SimulationData data)
	{
		var holz = data.Holz;
		var baumart = data.Baumart;

		var barkDamageFactor = holz.Rindenbeschädigungen / 100f;
		var radius = stamm.D_Stirn_mR * 0.5f * 0.001f;
		var rootHeight = baumart.IncludeRoots ? stamm.Stammfußhöhe * 0.01f : 0f;

		var parameters = new TrunkParameters()
		{
			DisplacementSize = Random.Range(baumart.MinNoiseSize, baumart.MaxNoiseSize),
			DisplacementStrength = Random.Range(baumart.MinNoiseStrength, baumart.MaxNoiseStrength),

			Length = stamm.Länge,
			RadiusMultiplier = radius - (barkDamageFactor * stamm.Rindenstärke * 0.001f),
			Taper = stamm.Abholzigkeit * 0.5f * 0.001f,
			BendingMultiplier = stamm.Krümmung * 0.001f,
			Ovality = stamm.Ovalität,
			BarkThickness = stamm.Rindenstärke * 0.001f * ( 1f - barkDamageFactor),
			BendingShape = GetBendingShape(holz.Krümmungsvarianten / 100f),
			RadialResolution = 6f,

			RootHeight = rootHeight,
			RootResolutionMultiplier = rootHeight > 0 ? 6f : 1f,
			RootRadius = (rootHeight > 0 ?
				Random.Range(baumart.MinRootRadiusMultiplier, baumart.MaxRootRadiusMultiplier) :
				1.0f),
			FlareNumber = (rootHeight > 0 ?
				Random.Range(baumart.MinRootFlareNumber, baumart.MaxRootFlareNumber) :
				0),

			BranchNumber = stamm.HasBranchStubs ? 
				(int)Mathf.Round(baumart.BranchStubNumberPerMeter * stamm.Länge) :
				0,
			BranchAngle = baumart.BranchStubAverageAngle,
			BranchRadius = baumart.BranchStubRadiusMultiplier,
			BranchStart = baumart.BranchStubMinHeight,
			BranchEnd = baumart.BranchStubMaxHeight,
			BranchMinLength = baumart.BranchStubMinLength,
			BranchMaxLength = baumart.BranchStubMaxLength
		};

		GameObject trunk = new GameObject(stamm.StammId);
		TrunkComponent mtrunk = trunk.AddComponent<TrunkComponent>();
		mtrunk.SetParameters(parameters);
		mtrunk.Generate();
		return trunk;
	}

	private BendingShape GetBendingShape(float probability)
	{
		float rand = Random.value;
		return (rand <= probability)? BendingShape.Banana : BendingShape.EndCurvature;
	}
}
