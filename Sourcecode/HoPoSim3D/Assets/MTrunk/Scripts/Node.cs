using System.Collections.Generic;
using UnityEngine;

namespace MTrunk
{
	public class Node
	{
		public List<Node> children;
		public Vector3 position;
		public Vector3 direction;
		public float radius;
		public float ovality;
		public float distanceFromOrigin; // total branch distance from the root to the Node
		public NodeType type; // Used to displace the tree after Growth
		public float positionInBranch; // relative position (0 to 1) of node in branch.

		public Node(Vector3 pos, float rad, Vector3 dir, float ovality, NodeType type = NodeType.Branch, float distancToOrigin=0)
		{
			this.position = pos;
			this.radius = rad;
			this.direction = dir;
			this.children = new List<Node>();
			this.ovality = ovality;
			this.type = type;
			this.distanceFromOrigin = distancToOrigin;
		}

		public Stack<Queue<TrunkPoint>> ToSplines()
		{
			Stack<Queue<TrunkPoint>> points = new Stack<Queue<TrunkPoint>>();
			points.Push(new Queue<TrunkPoint>());
			this.ToSplinesRec(points, Vector3.up);
			return points;
		}

		private void ToSplinesRec(Stack<Queue<TrunkPoint>> points, Vector3 parentDirection, float parentRadius = 0)
		{
			int n = children.Count;
			float rad = radius;
			if (type == NodeType.Trunk && n > 0)
			{
				rad = children[0].radius;
				direction = (children[0].position - position).normalized;
			}
			points.Peek().Enqueue(new TrunkPoint(position, direction, radius, type, parentDirection, distanceFromOrigin, parentRadius));
			if (n > 0)
				children[0].ToSplinesRec(points, direction);
			for (int i = 1; i < n; i++)
			{
				points.Push(new Queue<TrunkPoint>());
				children[i].ToSplinesRec(points, direction, rad);
			}
		}

		public void GetSelection(Queue<Node> selected, int selection, bool extremitiesOnly, ref float maxHeight)
		{
			if (!(extremitiesOnly && children.Count > 0))
			{
				selected.Enqueue(this);
				if (position.y > maxHeight)
					maxHeight = position.y;
			}
			foreach (Node child in children)
			{
				child.GetSelection(selected, selection, extremitiesOnly, ref maxHeight);
			}
		}

		public void GetSplitCandidates(Queue<Node> selected, float start, float remainingLength)
		{
			if (remainingLength <= 0)
				return;

			if (start <= positionInBranch && children.Count == 1)
			{
				positionInBranch = (positionInBranch - start) * (1 - start);
				selected.Enqueue(this);
			}

			for (int i = 0; i < children.Count; i++)
			{
				if (i == 0)
				{
					float dist = (children[i].position - position).magnitude;
					children[i].GetSplitCandidates(selected, start, remainingLength - dist);
				}
				else
				{
					children[i].GetSplitCandidates(selected, start, start);
				}
			}
		}

		public void DebugPosRec(List<Vector3> positions)
		{
			positions.Add(position);
			foreach (Node child in children)
			{
				child.DebugPosRec(positions);
			}
		}

	 
		public void Simplify(Node parent, float angleThreshold, float radiusThreshold)
		{
			if (radius < radiusThreshold)
			{
				children = new List<Node>();
				return;
			}

			Node nextParent = this;
			int n;
			if (children.Count > 0 && parent != null)
			{
				Vector3 v1 = position - parent.position;
				Vector3 v2 = children[0].position - position;
				if ((Vector3.Angle(v1, v2) < angleThreshold) && type != NodeType.Flare) // if true current Node must be removed
				{
					List<Node> parentChildren = new List<Node>() { children[0] }; // new childern for parent, with first child being self first child
					n = parent.children.Count;
					for (int i = 1; i < n; i++) // adding  original parent children
					{
						parentChildren.Add(parent.children[i]);
					}
					n = children.Count;
					for (int i = 1; i < n; i++)
					{
						parentChildren.Add(children[i]); // adding self children except firt one whih is already in list
					}
					parent.children = parentChildren;
					nextParent = parent;
				}
			}
			n = 0;
			foreach (Node child in children)
			{
				if (n == 0)
				{
					child.Simplify(nextParent, angleThreshold, radiusThreshold);
				}
				else
				{
					child.Simplify(null, angleThreshold, radiusThreshold);
				}
				n++;
			}
		}

		public float UpdatePositionInBranch(float distanceFromBranchOrigin=0, Node parent = null)
		{
			float dist = parent == null ? 0 : Vector3.Distance(parent.position, position);
			dist += distanceFromBranchOrigin;
			float totalDistance;
			if (children.Count == 0)
			{
				positionInBranch = 1f;
				totalDistance = dist;
			}
			else
			{
				totalDistance = children[0].UpdatePositionInBranch(dist, this);
				positionInBranch = dist / totalDistance;
				for (int i = 1; i < children.Count; i++)
				{
					children[i].UpdatePositionInBranch(0);
				}
			}
			return totalDistance;
		}
	}


	public enum NodeType {Flare, Trunk, Branch, FromTrunk }
}
