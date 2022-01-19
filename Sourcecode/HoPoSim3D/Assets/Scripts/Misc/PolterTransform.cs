using System.Linq;
using UnityEngine;

public static class PolterTransform
{
	public static Vector3[] Transform3dAverageDepth(Vector2[] points, Side side, float offset)
	{
		var z = GetAverageDepth(side, offset);
		return Transform3d(points, z);
	}

	public static Vector3[] Transform3dMaxDepth(Vector2[] points, Side side, float offset)
	{
		var z = GetMaxDepth(side, offset);
		return Transform3d(points, z);
	}

	private static Vector3[] Transform3d(Vector2[] points, float z)
	{
		return points.
			Select(p => new Vector3(p.x, p.y, z)).
			ToArray();
	}

	private static float GetAverageDepth(Side side, float offset)
	{
		var halfDepth = PolterManager.GetPolterAverageDepth() * 0.5f;

		var z = side == Side.FRONT ?
			- halfDepth - offset :
			halfDepth + offset;
		return z;
	}

	private static float GetMaxDepth(Side side, float offset)
	{
		var trunks = PolterManager.GetPolterTrunks(PolterManager.PolterTag);
		var bounds = BoundingBox.GetRendererBounds(trunks);

		var z = side == Side.FRONT ?
			bounds.min.z - offset :
			bounds.max.z + offset;
		return z;
	}


}
