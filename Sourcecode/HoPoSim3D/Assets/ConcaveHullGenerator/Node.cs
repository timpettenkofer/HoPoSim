using System;
using UnityEngine;

namespace ConcaveHull
{
	public class Node
	{
		public int id;
		public double x;
		public double y;
		public double cos; // Used for middlepoint calculations
		public Node(double x, double y, int id)
		{
			this.x = x;
			this.y = y;
			this.id = id;
		}

		public Vector2 ToVector2()
		{
			return new Vector2(Convert.ToSingle(x), Convert.ToSingle(y));
		}
	}
}