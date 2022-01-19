using UnityEngine;

public class TrunkMoveTracker : MonoBehaviour
{
	Rigidbody rb;

	public void Awake()
	{
		rb = gameObject.GetComponent<Rigidbody>();
	}

	public bool IsAwaken()
	{
		return !rb.IsSleeping();
	}

	public void WakeUp()
	{
		rb.WakeUp();
	}

	public void MakeKinematicIfPolterunterlage()
	{
		if (gameObject.tag == PolterManager.PolterunterlageTag)
			PolterManager.SetKinematic(gameObject);
	}
}
