using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MTrunk
{
	public class Trunk
	{
		public List<Node> stems;
		public Vector3[] verts;
		public Vector3[] normals;
		public Vector2[] uvs;
		public int[] triangles;
		public int subMeshCount;
		public List<int[]> submeshes;
		public Mesh[] colliders;
		public Color[] colors;
		public Transform treeTransform;

		public TrunkBasePoint bottom;
		public Vector3[] bottomOutline;
		public TrunkBasePoint top;
		public Vector3[] topOutline;


		public Trunk(Transform transform)
		{
			stems = new List<Node>();
			treeTransform = transform;
		}

		public void Create(TrunkParameters t)
		{
			var position = Vector3.down * t.HeightOffset;
			
			float epsilon = 10e-3f; // avoid artefact on trunk ends (1mm) 
			Vector3 direction = Vector3.up;

			float remainingLength = t.Length;

			var extremity = new Node(position, t.Radius.Evaluate(0) * t.RadiusMultiplier, Vector3.zero, t.Ovality, NodeType.Flare);
			stems.Add(extremity);

			while (remainingLength > 0)
			{
				float res = t.Resolution;
				NodeType type = NodeType.Trunk;
				Vector3 tangent = Vector3.Cross(direction, UnityEngine.Random.onUnitSphere);
				float currentHeight = t.Length - remainingLength;
				if (currentHeight < t.RootHeight)
				{
					tangent /= t.RootResolutionMultiplier * 2;
					res *= t.RootResolutionMultiplier;
					type = NodeType.Flare;
				}

				float branchLength = 1f / res;

				//Vector3 dir = randomness * tangent + extremity.direction * (1 - randomness);
				//Vector3 originAttractionVector = (position - extremity.position) * originAttraction;
				//originAttractionVector.y = 0;
				//dir += originAttractionVector;
				//dir.Normalize();

				if (remainingLength <= (branchLength + epsilon))
					branchLength = remainingLength;
				remainingLength -= branchLength;

				Vector3 dir = GetBendingVector(t.BendingShapes[(int)t.BendingShape], t.BendingMultiplier, currentHeight, currentHeight + branchLength, t.Length);
				if (extremity.direction == Vector3.zero)
					extremity.direction = dir;

				//Vector3 pos = extremity.position + dir * branchLength;
			
				float branchLengthScale = 1 / Vector3.Dot(direction, dir);
				Vector3 pos = extremity.position + dir * branchLength * branchLengthScale;
				
				float rad = t.Radius.Evaluate(1 - remainingLength / t.Length) * t.RadiusMultiplier;
				//if (length - remainingLength < rootHeight)
				//    rad += radiusMultiplier * rootRadius * rootShape.Evaluate((length - remainingLength) / rootHeight);
				Node child = new Node(pos, rad, dir, t.Ovality, type, distancToOrigin: t.Length-remainingLength);
				extremity.children.Add(child);
				extremity = child;
			}
		}

		public void Split(float taper, int expectedNumber, float splitAngle, float splitRadius, float startLength, float endLength, float spread, float flatten, int minLength, int maxLength)
		{
			Queue<Node> candidates = GetSplitCandidates(startLength, endLength);
			Vector3 direction = Vector3.zero;

			float totalLength = 0;
			foreach (Node ext in candidates)
				totalLength += (ext.position - ext.children[0].position).magnitude;

			float splitPerMeter = expectedNumber / totalLength;

			float remainder = 0f;

			foreach (Node ext in candidates)
			{
				float distToChild = (ext.position - ext.children[0].position).magnitude;

				float targetSplitNumber = distToChild * splitPerMeter + remainder;
				int number = (int)targetSplitNumber;
				remainder = targetSplitNumber - number;

				Vector3 randomVect = Random.onUnitSphere;
				if (flatten > 0)
					randomVect = Vector3.Lerp(randomVect, Vector3.up, flatten).normalized * (Random.Range(0, 1) * 2 - 1);
				Vector3 tangent = Vector3.Cross(randomVect, ext.direction);
				Quaternion rot = Quaternion.AngleAxis(360f / number, ext.direction);
				Vector3 spreadDirection = ext.children[0].direction;

				var blend_start = Random.Range(0f, 0.95f);
				for (int i = 0; i < number; i++)
				{
					float blend = blend_start + Random.Range(0f, 0.05f) * spread; // Random.value * spread;
					float rad = Mathf.Lerp(ext.radius * splitRadius, ext.children[0].radius * splitRadius, blend);

					Vector3 pos = ext.position + spreadDirection * blend * distToChild;
					//Vector3 dir = Vector3.LerpUnclamped(ext.direction, tangent, splitAngle * (1 - ext.positionInBranch / 2)).normalized + direction;
					// get a more deterministic branch's angle
					Vector3 dir = Vector3.LerpUnclamped(ext.direction, tangent, splitAngle).normalized + direction;

					float radiusFactor = Mathf.Clamp01(rad);
					var radius = Mathf.Max(ext.radius * radiusFactor + rad * (1 - radiusFactor), 0.02f);


					float radiusAtHeight = Splines.GetTaperedRadius(taper, pos.y, ext.radius); 
					var branchLength = radiusAtHeight + Random.Range(minLength, maxLength) * 0.001f;

					var branchEndPosition = pos + dir * branchLength;
					if (branchEndPosition.y + radius <= endLength) // avoid branches that project beyond the top of the log
					{
						Node branchBase = new Node(pos, radius, dir, ext.ovality, distancToOrigin: ext.distanceFromOrigin + blend * distToChild);

						if (ext.type == NodeType.Trunk || ext.type == NodeType.Flare)
							branchBase.type = NodeType.FromTrunk;

						Node branchEnd = new Node(branchEndPosition, radius, dir, ext.ovality, distancToOrigin: ext.distanceFromOrigin + blend * distToChild);

						branchBase.children.Add(branchEnd);

						//child0.growth.canBeSplit = false;
						ext.children.Add(branchBase);
						tangent = rot * tangent;
					}
				}
			}
		}

		public static float GetTaperedRadius(TrunkParameters t, float y, float radius)
		{
			float tapering = y * t.Taper;
			float taperedRadius = Mathf.Max(0, radius - tapering);
			return taperedRadius;
		}

		public void AddBranches(TrunkParameters t)
		{
			if (t.BranchNumber > 0)
			{
				int number = t.BranchNumber;
				float angle = t.BranchAngle;
				float radius = t.BranchRadius;
				float start = t.BranchStart;
				float end = t.BranchEnd * t.Length;
				int minLength = t.BranchMinLength;
				int maxLength = t.BranchMaxLength;
				float flatten = 0f;

				Split(t.Taper, number, angle, radius, start, end, 1, flatten, minLength, maxLength);
			}
		}

		private Queue<Node> GetSelection(int selection, bool extremitiesOnly, ref float maxHeight)
		{
			Queue<Node> selected = new Queue<Node>();
			foreach (Node stem in stems)
			{
				stem.GetSelection(selected, selection, extremitiesOnly, ref maxHeight);
			}
			return selected;
		}


		private Queue<Node> GetSplitCandidates(float start, float remainingLength)
		{
			Queue<Node> selected = new Queue<Node>();
			foreach (Node stem in stems)
			{
				stem.UpdatePositionInBranch();
				stem.GetSplitCandidates(selected, start, remainingLength);
			}
			return selected;
		}

		private Vector3 GetBendingVector(AnimationCurve bending, float multiplier, float from, float to, float scale)
		{
			var fromValue = bending.Evaluate(from / scale);
			var toValue = bending.Evaluate(to / scale);
			var dir = new Vector3(multiplier * (toValue - fromValue), (to - from) / scale, 0);
			dir.Normalize();
			return dir;
		}

		public void Simplify(float angleThreshold, float radiusTreshold)
		{

			foreach (Node stem in stems)
			{
				stem.Simplify(null, angleThreshold, radiusTreshold);
			}
		}

		public void GenerateMeshData(TrunkParameters trunk)
		{
			Stack<Queue<TrunkPoint>> treePoints = new Stack<Queue<TrunkPoint>>();
			foreach (Node stem in stems)
			{
				Stack<Queue<TrunkPoint>> newPoints = stem.ToSplines();
				while (newPoints.Count > 0)
				{
					treePoints.Push(newPoints.Pop());
				}
			}
			Splines splines = new Splines(treePoints);

			//Queue<TrunkPoint> trunkPoints = stem.ToSplines(); // new Queue<TrunkPoint>();
			//Queue<TrunkPoint> newPoints = stem.ToSplines();
			//while(newPoints.Count > 0)
			//{
			//	trunkPoints.Push(newPoints.Pop());
			//}
			//Splines splines = new Splines(trunkPoints);
			splines.GenerateMeshData(trunk, 7* trunk.RadialResolution, 3);
			verts = splines.verts.ToArray();
			bottom = splines.bottom;
			top = splines.top;
			bottomOutline = splines.bottomOutline.ToArray();
			topOutline = splines.topOutline.ToArray();
			normals = splines.normals.ToArray();
			uvs = splines.uvs.ToArray();
			triangles = splines.triangles.ToArray();
			colliders = splines.colliders.ToArray();
			subMeshCount = splines.submeshes.Count();
			submeshes = new List<int[]>(subMeshCount);
			for (int i = 0; i < subMeshCount; ++i)
				submeshes.Add(splines.submeshes[i].ToArray());
			colors = splines.colors.ToArray();
		}

		public List<Vector3> DebugPositions()
		{
			List<Vector3> positions = new List<Vector3>();
			foreach (Node stem in stems)
			{
				stem.DebugPosRec(positions);
			}
			return positions;
		}

		



	}

}
