using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
	public static class Area
	{
		public static float ComputeArea(IList<Vector2> vertices)
		{
			var area = ComputeSignedArea(vertices);
			return Mathf.Abs(area);
		}

		public static float ComputeSignedArea(IList<Vector2> vertices)
		{
			if (vertices.Count < 3)
				return 0;

			var area = GetDeterminant(vertices[vertices.Count - 1].x, vertices[vertices.Count - 1].y, vertices[0].x, vertices[0].y);
			for (int i = 1; i < vertices.Count; i++)
			{
				area += GetDeterminant(vertices[i - 1].x, vertices[i - 1].y, vertices[i].x, vertices[i].y);
			}
			return area * 0.5f;
		}

		public static float ComputeSignedArea(Vector2 v1, Vector2 v2, Vector2 v3)
		{
			var area = GetDeterminant(v3.x, v3.y, v1.x, v1.y);
			area += GetDeterminant(v1.x, v1.y, v2.x, v2.y);
			area += GetDeterminant(v2.x, v2.y, v3.x, v3.y);
			return area * 0.5f;
		}

		static float GetDeterminant(float x1, float y1, float x2, float y2)
		{
			return x1 * y2 - x2 * y1;
		}
	}
}
