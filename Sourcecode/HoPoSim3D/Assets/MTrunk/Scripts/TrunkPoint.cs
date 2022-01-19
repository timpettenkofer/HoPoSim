using UnityEngine;

namespace MTrunk
{
	public struct TrunkPoint
	{
		public Vector3 position, direction;
		public float radius;
		public NodeType type;
		public Vector3 parentDirection;
		public float parentRadius;
		public float distanceFromOrigin;

		public TrunkPoint(Vector3 position, Vector3 direction, float radius, NodeType type, Vector3 parentDirection, float distanceFromOrigin, float parentRadius)
		{
			this.position = position;
			this.direction = direction;
			this.radius = radius;
			this.type = type;
			this.parentDirection = parentDirection;
			this.distanceFromOrigin = distanceFromOrigin;
			this.parentRadius = parentRadius;
		}
	}
}
