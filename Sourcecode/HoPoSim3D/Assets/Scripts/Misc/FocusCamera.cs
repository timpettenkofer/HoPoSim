using UnityEngine;

public class FocusCamera : MonoBehaviour
{
	public KeyCode Key_Front_Camera_Focus;
	public KeyCode Key_Back_Camera_Focus;
	public GameObject target;
	public float sideMargin = 0.2f;
	

	void Start() { Init(); }

	public void Init()
	{
		if (Key_Back_Camera_Focus == KeyCode.None)
			Key_Back_Camera_Focus = KeyCode.H;
		if (Key_Front_Camera_Focus == KeyCode.None)
			Key_Front_Camera_Focus = KeyCode.V;
	}

	void LateUpdate()
	{
		if (Input.GetKeyDown(Key_Front_Camera_Focus))
			Focus(Side.FRONT);
		else if (Input.GetKeyDown(Key_Back_Camera_Focus))
			Focus(Side.BACK);
	}

	public void Focus(Side side, float? aspectRatio = null)
	{
		var camera = Camera.main;
		var bounds = GetTargetBoundingBox();

		var radAngle = camera.fieldOfView * Mathf.Deg2Rad;
		var aspect = aspectRatio.HasValue ? aspectRatio.Value : camera.aspect;
		float distance = ComputeMinimumDistance(camera, bounds, radAngle, aspect);

		var dir = side == Side.FRONT ? Vector3.back : Vector3.forward;
		var position = bounds.center + (bounds.extents.z + distance) * dir;
		camera.transform.position = position;

		var angle = side == Side.FRONT ? 0 : -180;
		camera.transform.eulerAngles = new Vector3(0, angle, 0);
	}

	private Bounds GetTargetBoundingBox()
	{
		var bbox = BoundingBox.GetColliderBounds(target);
		if (bbox.size != Vector3.zero)
			return bbox;
		// no polterunterlage, estimate bounds from configuration
		bbox = BoundingBox.GetPolterBounds(ConfigurationHelper.SimulationData.Poltermaße); 
		bbox.Expand(1.3f);
		return bbox;
	}

	private float ComputeMinimumDistance(Camera camera, Bounds bounds, float radAngle, float aspect)
	{
		var radHFOV = 2 * Mathf.Atan(Mathf.Tan(radAngle / 2) * aspect);
		float objectWidth = bounds.size.x + 2 * sideMargin;
		float hdistance = objectWidth / (2 * Mathf.Tan(radHFOV / 2));

		var radVFOV = Mathf.Deg2Rad * camera.fieldOfView;
		float objectHeight = bounds.size.y + 2 * sideMargin;
		float vdistance = objectHeight / (2 * Mathf.Tan(radVFOV / 2));

		var distance = Mathf.Max(hdistance, vdistance);
		return distance;
	}
}

