using HoPoSim.IPC.DAO;
using System;
using UnityEngine;

public class TrapezBuilder
{
	private GameObject Left;
	private GameObject Right;
	private GameObject Front;
	private GameObject Back;

	#region Trapez
	public void CreateTrapez()
	{
		var trapez = GameObject.Find("Trapez");
		if (trapez != null)
			UnityEngine.Object.Destroy(trapez);

		trapez = new GameObject() { name = "Trapez" };

		Right = CreateTrapezSide("Right", trapez, body => { body.useGravity = false; body.isKinematic = true; });
		Left = CreateTrapezSide("Left", trapez, body => { body.useGravity = false; body.isKinematic = true; });
		Front = CreateTrapezSide("Front", trapez, body => { body.useGravity = false; body.isKinematic = true; });
		Back = CreateTrapezSide("Back", trapez, body => { body.useGravity = false; body.isKinematic = true; });

		PolterManager.SetPolterTools(trapez);

		Right.GetComponent<MeshRenderer>().enabled = false;
		Left.GetComponent<MeshRenderer>().enabled = false;
		Front.GetComponent<MeshRenderer>().enabled = false;
		Back.GetComponent<MeshRenderer>().enabled = false;
	}

	private GameObject CreateTrapezSide(string name, GameObject parent, Action<Rigidbody> rigidBodyConfigurator = null)
	{
		var side = GameObject.CreatePrimitive(PrimitiveType.Cube);
		side.transform.localScale = new Vector3(0.01f, 1, 1);
		side.name = name;

		var rigidBody = side.AddComponent<Rigidbody>();
		rigidBodyConfigurator?.Invoke(rigidBody);

		var colliders = side.GetComponentsInChildren<Collider>();
		foreach (var collider in colliders)
			collider.material = (PhysicMaterial)Resources.Load("PhysicMaterials/LowFriction");

		side.transform.parent = parent.transform;
		return side;
	}

	public void SetSideAngle(float degrees)
	{
		Right.transform.localEulerAngles = new Vector3(-(90 - degrees), 90, 0);
		Left.transform.localEulerAngles = new Vector3(-(90 - degrees), 270, 0);
		Front.transform.localEulerAngles = new Vector3(0, 0, 0);
		Back.transform.localEulerAngles = new Vector3(0, 0, 0);
	}

	public void ResetSideAngle(SimulationData data, float xOffset, float angle)
	{
		var length = data.Poltermaße.MinimumPolterlänge;
		Right.transform.RotateAround(new Vector3(length + xOffset, 0, 0), Vector3.back, Left.transform.eulerAngles.x);
		Right.transform.RotateAround(new Vector3(length + xOffset, 0, 0), Vector3.back, angle);
		Left.transform.RotateAround(new Vector3(xOffset, 0, 0), Vector3.back, -Left.transform.eulerAngles.x);
		Left.transform.RotateAround(new Vector3(xOffset, 0, 0), Vector3.back, -angle);
	}

	public void SetTrapezInitialBounds(float length, float angle, float xOffset, float maxDepth)
	{
		var angleInRadians = Mathf.Deg2Rad * angle;

		var side_length = 0.5f * length / (float)Math.Cos(angleInRadians);
		var height = 0.5f * length * (float)Math.Tan(angleInRadians);

		SetSideAngle(angle);

		Right.transform.localScale = new Vector3(maxDepth, side_length, 0);
		Left.transform.localScale = new Vector3(maxDepth, side_length, 0);

		Front.transform.localScale = new Vector3(length, height, 0);
		Back.transform.localScale = new Vector3(length, height, 0);

		Left.transform.localPosition = new Vector3(xOffset + length * 0.25f, height * 0.5f, 0);
		Right.transform.localPosition = new Vector3(xOffset + length * 0.75f, height * 0.5f, 0);

		var maxDepthWithTolerance = Mathf.Min(maxDepth * 1.04f, maxDepth + 0.15f);
		Front.transform.localPosition = new Vector3(xOffset + length * 0.5f, height * 0.5f, -0.5f * maxDepthWithTolerance);
		Back.transform.localPosition = new Vector3(xOffset + length * 0.5f, height * 0.5f, 0.5f * maxDepthWithTolerance);
	}

	public float MaximumHeight(SimulationData data)
	{
		var half_length = 0.5f * data.Poltermaße.MinimumPolterlänge;
		var steigungswinkelInRadians = Mathf.Deg2Rad * data.Poltermaße.Steigungswinkel;
		var maxHeight = Mathf.Tan(steigungswinkelInRadians) * half_length;
		return maxHeight;
	}
	#endregion
}

