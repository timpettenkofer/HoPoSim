using HoPoSim.IPC.DAO;
using UnityEngine;

[RequireComponent(typeof(MeshVolume))]
public class Inspector : MonoBehaviour
{
	public Stamm Parametrization { get; set; }

	public float Radius;
	public float Length;

	public float Exact_Cylinder_Volume;
	public float VolumeErrorPercent;


	public float Rindenanteil;


	public float FmmR
	{
		get
		{
			if (_FmmR == -1)
				Compute();
			return _FmmR;
		}
	}
	public float _FmmR = -1;

	public float FmoR
	{
		get
		{
			if (_FmoR == -1)
				Compute();
			return _FmoR;
		}
	}
	public float _FmoR = -1;

	void Start()
	{
		if (_FmmR == -1)
			Compute();
	}

	void Compute()
	{
		var meshVolume = gameObject.GetComponent<MeshVolume>();
		_FmmR = meshVolume.FmmR;
		_FmoR = meshVolume.FmoR;

		Radius = DiameterInMillimeterToRadiusInMeter(Parametrization.D_Stirn_mR);
		Length = Parametrization.Länge;
		Exact_Cylinder_Volume = Mathf.PI * Radius * Radius * Length;

		VolumeErrorPercent = 100.0f - ((_FmmR * 100.0f) / Exact_Cylinder_Volume);

		Rindenanteil = GetRindenanteil(_FmmR, _FmoR);
	}

	private static float GetRindenanteil(float FmmR, float FmoR)
	{
		return FmmR != 0.0f ? ((FmmR - FmoR) / FmmR) * 100.0f : -1;
	}

	private float FromMillimeterToMeter(int value)
	{
		return value * 0.001f;
	}

	private float DiameterInMillimeterToRadiusInMeter(int diameter)
	{
		return FromMillimeterToMeter(diameter) * 0.5f;
	}
}
