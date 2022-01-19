using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MTrunk
{
	public class TrunkBasePoint
	{
		public TrunkBasePoint(Vector3 point, Vector3 normal, float radius)
		{
			this.position = point;
			this.normal = normal;
			this.radius = radius;
		}

		public Vector3 position;
		public Vector3 normal;
		public float radius;
	}

	public class Splines
	{
		Stack<Queue<TrunkPoint>> splines;
		public List<Vector3> verts;
		public TrunkBasePoint top;
		public List<Vector3> topOutline;
		public TrunkBasePoint bottom;
		public List<Vector3> bottomOutline;
		public Queue<Vector2> uvs;
		public Queue<Vector3> normals;
		public Queue<int> triangles;
		public Queue<Color> colors;
		public Queue<int>[] submeshes;
		public Queue<Mesh> colliders;

		public const int bark_mesh = 0;
		public const int section_mesh = 1;

		private static SimplexNoiseGenerator noiseGenerator = new SimplexNoiseGenerator(new int[] { 0x16, 0x38, 0x32, 0x2c, 0x0d, 0x13, 0x07, 0x2a });

		public Splines(Stack<Queue<TrunkPoint>> points)
		{
			splines = points;
			verts = new List<Vector3>();
			uvs = new Queue<Vector2>();
			normals = new Queue<Vector3>();
			triangles = new Queue<int>();
			colors = new Queue<Color>();
			submeshes = new Queue<int>[2] { new Queue<int>(), new Queue<int>() };
			colliders = new Queue<Mesh>();
			topOutline = new List<Vector3>();
			bottomOutline = new List<Vector3>();
		}

		public void GenerateMeshData(TrunkParameters t, float resolutionMultiplier, int minResolution)
		{
			Queue<int> trianglesWithBadNormals = new Queue<int>(); // Triangles whose verts have been displaced, needing their normals to be recalculated
			Queue<Vector2Int> duplicatedVertexIndexes = new Queue<Vector2Int>(); // Used when recalculating normals to avoid shading seams
																				 //if (spline == null)
																				 //	return;
			int[] trunkTriangles = null;
			Vector3[] trunkVertices = null;
			Queue<BarycentricCoordinates> projectedVertices = new Queue<BarycentricCoordinates>();
			bool isTrunk = true;

			while (splines.Count > 0) // drawing each node inside the trunk
			{
				Queue<TrunkPoint> spline = splines.Pop();
				int lastResolution = -1;
				float uv_height = 0f;

				while (spline.Count > 0) // drawing each node inside the branch
				{
					TrunkPoint point = spline.Dequeue();
					point.radius = Mathf.Max(point.radius, 0.001f);
					int resolution = getResolution(point, minResolution, t.RootRadius, t.RootHeight, resolutionMultiplier);
					bool isFirstLoop = lastResolution == -1;

					if (isFirstLoop && (point.type == NodeType.Trunk || point.type == NodeType.Flare))
					{
						AddTrunkEnd(t, uv_height, point, resolution, false);
						float taperedRadius = GetTaperedRadius(t, point);
						bottom = new TrunkBasePoint(point.position, -point.direction, taperedRadius);
					}

					Vector3[] newVertices;
					int n = verts.Count;

					if (lastResolution == -1 && point.parentRadius != 0 && point.type == NodeType.FromTrunk)
					{
						float taperedRadius = GetTaperedRadius(t.Taper, point.position.y, point.parentRadius);
						newVertices = AddCircleWrapped(point.position, point.direction, point.radius, resolution, taperedRadius, point.parentDirection, t.DisplacementStrength);
						ProjectBranchesOnTrunk(trunkTriangles, trunkVertices, ref newVertices, projectedVertices, n);
					}
					else
					{
						newVertices = AddSection(point, t, resolution, uv_height * t.SpinAmount * point.radius);
						//newVerts = AddCircle(point.position, point.direction, point.radius, resolution, uv_height * spinAmount * point.radius);
					}


					duplicatedVertexIndexes.Enqueue(new Vector2Int(n, n + resolution - 1));
					List<Vector3> colliderVertices = new List<Vector3>();
					Queue<int> colliderTriangles = new Queue<int>();

					BridgeLoops(t, newVertices, point, lastResolution, uv_height, n, getFillingGapRate(lastResolution, resolution), trianglesWithBadNormals, colliderVertices, colliderTriangles);
					if (!isFirstLoop)
					{
						var collider = new Mesh
						{
							vertices = colliderVertices.ToArray(),
							triangles = colliderTriangles.ToArray()
						};
						colliders.Enqueue(collider);
					}

					if (spline.Count > 0)
					{
						float rad = point.radius;
						if (point.type == NodeType.Flare)
						{
							var rootHeight = t.RootHeight > 0.0f ? point.position.y / t.RootHeight : 1f;
							rad += t.RootRadius * t.RootShape.Evaluate(rootHeight) * t.RadiusMultiplier;
						}
						uv_height += (point.position - spline.Peek().position).magnitude / rad;
					}
					else
					{
						// last loop
						AddTrunkEnd(t, uv_height, point, resolution, true);
						float taperedRadius = GetTaperedRadius(t, point);
						if (point.type == NodeType.Trunk)
							top = new TrunkBasePoint(point.position, point.direction, taperedRadius);
					}

					lastResolution = resolution;
				}
					if (isTrunk)
					{
						trunkTriangles = triangles.ToArray();
						trunkVertices = verts.ToArray();
						isTrunk = false;
					}
				}
			
			RecalculateNormals(duplicatedVertexIndexes, trianglesWithBadNormals, projectedVertices);
		}

		public static Vector3[] AddCircleWrapped(Vector3 position, Vector3 direction, float radius, int resolution, float parentRadius, Vector3 parentDirection, float displacementStrength)
		{
			Vector3[] verts = new Vector3[resolution];
			Vector3 orthoDir = Vector3.ProjectOnPlane(direction, parentDirection).normalized;
			Quaternion rotation = Quaternion.FromToRotation(Vector3.up, orthoDir);
			Vector3 tangent = Vector3.Cross(parentDirection, orthoDir);
			float scale = Mathf.Sqrt(Mathf.Pow(Mathf.Tan(90 - Vector3.Angle(direction, parentDirection) * Mathf.PI / 180), 2) + 1);
			scale = Mathf.Min(scale, 4f) / 4;
			Matrix4x4 scaleMat = Matrix4x4.identity;
			scaleMat[0, 0] += parentDirection.x * scale;
			scaleMat[1, 1] += parentDirection.y * scale;
			scaleMat[2, 2] += parentDirection.z * scale;

			for (int i = 0; i < resolution; i++)
			{
				float angle = Mathf.PI * 2 * ((float)i / (resolution - 1));
				Vector3 vert = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
				vert = (rotation * vert);
				float wrap = Mathf.Sin(Mathf.Acos(Mathf.Clamp01(Mathf.Abs(Vector3.Dot(vert, tangent)) / parentRadius))) * parentRadius;
				vert = scaleMat.MultiplyPoint(vert);
				vert += orthoDir * wrap;
				vert += position + parentDirection * Vector3.Dot(direction, parentDirection) * parentRadius;
				verts[i] = vert;
			}
			return verts;
		}

		public void ProjectBranchesOnTrunk(int[] trunkTriangles, Vector3[] trunkVertices, ref Vector3[] vertices, Queue<BarycentricCoordinates> projectedVertices, int verticesCount)
		{
			Vector3[] centers = new Vector3[trunkTriangles.Length / 3];
			for (int i = 0; i < centers.Length; i++)
				centers[i] = (trunkVertices[trunkTriangles[i * 3]] + trunkVertices[trunkTriangles[i * 3 + 1]] + trunkVertices[trunkTriangles[i * 3 + 2]]) / 3;

			for (int i = 0; i < vertices.Length; i++)
			{
				Vector3 vert = vertices[i];
				int closestTriangle = 0;
				float minDist = Mathf.Infinity;
				for (int j = 0; j < centers.Length; j++)
				{
					float dist = Vector3.SqrMagnitude(centers[j] - vert);
					if (dist < minDist)
					{
						minDist = dist;
						closestTriangle = j * 3;
					}
				}

				Vector3 projectedVert = ClosestPointOnTriangle(trunkVertices[trunkTriangles[closestTriangle]], trunkVertices[trunkTriangles[closestTriangle + 1]], trunkVertices[trunkTriangles[closestTriangle + 2]], vertices[i]);
				Vector3 coordinates = Barycentric(projectedVert, trunkVertices[trunkTriangles[closestTriangle]], trunkVertices[trunkTriangles[closestTriangle + 1]], trunkVertices[trunkTriangles[closestTriangle + 2]]);
				projectedVertices.Enqueue(new BarycentricCoordinates(verticesCount + i, trunkTriangles[closestTriangle], trunkTriangles[closestTriangle], trunkTriangles[closestTriangle], coordinates));
				vertices[i] = projectedVert;
			}
			//vertices[i] = centers[closestTriangle / 3];
		}

		private Vector3 Barycentric(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
		{
			Vector3 v0 = b - a;
			Vector3 v1 = c - a;
			Vector3 v2 = p - a;
			float d00 = Vector3.Dot(v0, v0);
			float d01 = Vector3.Dot(v0, v1);
			float d11 = Vector3.Dot(v1, v1);
			float d20 = Vector3.Dot(v2, v0);
			float d21 = Vector3.Dot(v2, v1);
			float denom = d00 * d11 - d01 * d01;

			Vector3 coordinates = Vector3.zero;
			coordinates.y = (d11 * d20 - d01 * d21) / denom;
			coordinates.z = (d00 * d21 - d01 * d20) / denom;
			coordinates.x = 1.0f - coordinates.y - coordinates.z;

			return coordinates;
		}
		public Vector3 ClosestPointOnTriangle(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 point)
		{
			Vector3 normal = Vector3.Cross(v1 - v2, v1 - v3);
			return Vector3.ProjectOnPlane(point - v1, normal) + v1;
		}

		private static float GetTaperedRadius(TrunkParameters t, TrunkPoint point)
		{
			return GetTaperedRadius(t.Taper, point.position.y, point.radius);
		}

		public static float GetTaperedRadius(float taper, float y, float radius)
		{
			float tapering = y * taper;
			float taperedRadius = Mathf.Max(0, radius - tapering);
			return taperedRadius;
		}

		private void AddTrunkEnd(TrunkParameters t, float uv_height, TrunkPoint point, int resolution, bool isTopEnd)
		{
			var normal = isTopEnd? point.direction : -point.direction;

			AddVertex(point.position, normal, new Vector2(0.5f, 0.5f), Color.white);

			int centerIndex = verts.Count - 1;

			var vertices = AddSection(point, t, resolution, uv_height * t.SpinAmount * point.radius, true);
			if (point.type == NodeType.Trunk || point.type == NodeType.Flare)
			{
				if (isTopEnd)
					topOutline.AddRange(vertices);
				else
					bottomOutline.AddRange(vertices);
			}

			var bounds = GetBounds(vertices);
			foreach (var vertex in vertices)
			{
				var u = (vertex.x - bounds.min.x) / bounds.size.x;
				var v = (vertex.z - bounds.min.z) / bounds.size.z;
				AddVertex(vertex, normal, new Vector2(u, v), Color.white);
			}

			int fromIndex = centerIndex + 1;
			int toIndex = centerIndex + resolution;

			for (int i = fromIndex + 1; i <= toIndex; i++)
				if (isTopEnd)
					AddTriangleToMeshes(i, i - 1, centerIndex, section_mesh);
				else
					AddTriangleToMeshes(i - 1, i, centerIndex, section_mesh);
			if (isTopEnd)
				AddTriangleToMeshes(fromIndex, toIndex, centerIndex, section_mesh);
			else
				AddTriangleToMeshes(toIndex, fromIndex, centerIndex, section_mesh);
		}

		private static Bounds GetBounds(IEnumerable<Vector3> vertices)
		{
			if (!vertices.Any())
				return new Bounds();
			Bounds bounds = new Bounds(vertices.First(), Vector3.zero);
			foreach (var vertex in vertices)
				bounds.Encapsulate(vertex);
			return bounds;

		}

		public static Vector3[] AddSection(TrunkPoint point, TrunkParameters t, int resolution, float spinAngle, bool applyDisplacements = false)
		{
			Vector3[] verts = new Vector3[resolution];
			Quaternion rotation = Quaternion.FromToRotation(Vector3.up, point.direction);
			for (int i = 0; i < resolution; i++)
			{
				float angle = Mathf.PI * 2 * ((float)i / (resolution - 1)) + spinAngle;
				float taperedRadius = GetTaperedRadius(t, point);
				Vector3 vert = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle) * t.Ovality) * taperedRadius;
				verts[i] = rotation * vert + point.position;
				if (applyDisplacements)
				{
					bool badNormals = false;
					Vector3 normal = (verts[i] - point.position).normalized;
					verts[i] = ApplyTrunkDisplacement(verts[i], t, point, ref badNormals);
					verts[i] = ApplyFlaresDisplacement(verts[i], t, point, resolution, i, normal);
				}
			}
			return verts;
		}

		void BridgeLoops(TrunkParameters t, Vector3[] loop, TrunkPoint point, int lastResolution, float uvHeight, int n, int fillingGapRate, Queue<int> trianglesWithBadNormals, List<Vector3> colliderVertices, Queue<int> colliderTriangles)
		{
			int resolution = loop.Length;
			int gaps = lastResolution - resolution; //difference between the two loops

			if (n > 0)
				colliderVertices.AddRange(verts.Skip(n - lastResolution));
			colliderVertices.AddRange(loop);

			for (int i = 0; i < resolution; i++)
			{
				Vector3 vert = loop[i];
				Vector3 normal = (vert - point.position).normalized;

				Vector3 nrm = (vert - point.position).normalized;
				float uvOffset = 0f;
				if (lastResolution == -1)
				{
					Vector3 orthoDir = Vector3.ProjectOnPlane(point.direction, point.parentDirection).normalized;
					Vector3 center = (loop[0] + loop[resolution / 2]) / 2 - orthoDir * point.parentRadius;
					Vector3 pos = Vector3.Project(vert - center, point.parentDirection) + center;
					nrm = (vert - pos).normalized;
				}
				bool badNormals = false;
				vert = ApplyTrunkDisplacement(vert, t, point, ref badNormals);
				vert = ApplyFlaresDisplacement(vert, t, point, resolution, i, normal);

				verts.Add(vert);
				normals.Enqueue(nrm);
				uvs.Enqueue(new Vector2(i * 1f / (resolution - 1), uvHeight / 3.2f + uvOffset));

				if (lastResolution > 0)
				{
					if (i > 0)
					{
						if (badNormals)
						{
							trianglesWithBadNormals.Enqueue(triangles.Count);
							trianglesWithBadNormals.Enqueue(triangles.Count + 3);
						}
						AddTriangleToMeshes(n - lastResolution + i - 1, n + i - 1, n - lastResolution + i, bark_mesh);
						AddTriangleToMeshes(n + i - 1, n + i, n - lastResolution + i, bark_mesh);
						AddTriangle(colliderTriangles, i - 1, lastResolution + i - 1, i);
						AddTriangle(colliderTriangles, lastResolution + i -1, lastResolution + i, i);

						if (i % fillingGapRate == 0 && gaps > 0) // filling a gap
						{
							if (badNormals)
								trianglesWithBadNormals.Enqueue(triangles.Count);
							AddTriangleToMeshes(n - lastResolution + i, n + i, n - lastResolution + i + 1, bark_mesh);
							AddTriangle(colliderTriangles, i, lastResolution + i, i + 1);
							gaps--;
							lastResolution--;
						}
					}
				}

				Color col = new Color(point.distanceFromOrigin / 10, 0, 0);
				colors.Enqueue(col);
			}
			if (gaps > 0) // Fill eventual remaining gap
			{
				AddTriangleToMeshes(n - lastResolution + resolution - 1, n + resolution - 1, n - lastResolution + resolution, bark_mesh);
				AddTriangle(colliderTriangles, resolution - 1, lastResolution + resolution -1, resolution);
			}
		}

		private static Vector3 ApplyFlaresDisplacement(Vector3 vert, TrunkParameters t, TrunkPoint point, int resolution, int i, Vector3 normal)
		{
			if (point.type == NodeType.Flare) // root flares displacement
			{
				float angle = i * 1f / (resolution - 1) * 2 * Mathf.PI;
				var radiusHeight = t.RootHeight > 0.0f ? point.position.y / t.RootHeight : 0.0f;
				float displacement = Mathf.Abs(Mathf.Sin(angle * t.FlareNumber / 2f)) * (t.RootRadius - 1f) * t.RootShape.Evaluate(radiusHeight) * t.RadiusMultiplier;
				vert += normal * displacement * Random.Range(0.9f, 1f);
			}

			return vert;
		}

		private static Vector3 ApplyTrunkDisplacement(Vector3 vert, TrunkParameters t, TrunkPoint point, ref bool badNormals)
		{
			var displacementSize = point.type == NodeType.Flare? t.DisplacementSize : 0f;
			var displacementStrength = point.type == NodeType.Flare ? t.DisplacementStrength : 0f;

			if (point.type == NodeType.Trunk || point.type == NodeType.Flare) //Trunk displacement
			{
				vert += noiseGenerator.noiseGradient(vert * displacementSize / t.RadiusMultiplier, flat: true) / 15f * point.radius * displacementStrength;
				badNormals = true;
			}
			return vert;
		}

		int getFillingGapRate(int lastResolution, int resolution)
		{
			int gaps = lastResolution - resolution; //difference between the two loops
			int fillingGapRate = int.MaxValue; //rate at which an additional triangle must be created
			if (gaps > resolution)
				resolution = gaps; // increase resolution when there are too many gaps to fill
			if (gaps > 0)
				fillingGapRate = resolution / gaps;

			return fillingGapRate;
		}

		int getResolution(TrunkPoint point, int minResolution, float rootRadius, float rootHeight, float resolutionMultiplier)
		{
			int resolution = (int)((point.radius) * resolutionMultiplier * 7);

			//if (point.type == NodeType.Flare)
			//{
			//	var radiusHeight = rootHeight > 0.0 ? point.position.y / rootHeight : 0.0f;
			//	resolution += (int)(rootRadius * (Mathf.Pow(1 - Mathf.Max(0, radiusHeight), 2)) * resolutionMultiplier * 3);
			//}

			if (resolution < minResolution)
				resolution = minResolution;
			//resolution++;
			return resolution;
		}

		public void RecalculateNormals(Queue<Vector2Int> duplicatedVerts, Queue<int> selectedTriangles, Queue<BarycentricCoordinates> projectedVertices)
		{
			Vector3[] newNormals = normals.ToArray();
			HashSet<int> overidenIndexes = new HashSet<int>();
			Vector3[] Verts = verts.ToArray();
			int[] Tris = triangles.ToArray();
			int n = Tris.Length;
			foreach (int i in selectedTriangles)
			{
				int a = Tris[i];
				int b = Tris[i+1];
				int c = Tris[i+2];

				if (!overidenIndexes.Contains(a))
				{
					newNormals[a] = Vector3.zero;
					overidenIndexes.Add(a);
				}
				if (!overidenIndexes.Contains(b))
				{
					newNormals[b] = Vector3.zero;
					overidenIndexes.Add(b);
				}
				if (!overidenIndexes.Contains(c))
				{
					newNormals[c] = Vector3.zero;
					overidenIndexes.Add(c);
				}

				Vector3 nrm = Vector3.Cross(Verts[b] - Verts[a], Verts[c] - Verts[a]);

				newNormals[a] += nrm;
				newNormals[b] += nrm;
				newNormals[c] += nrm;
			}
			
			foreach(int i in overidenIndexes)
			{
				newNormals[i].Normalize();
			}

			foreach (Vector2Int indexes in duplicatedVerts)
			{
				int x = indexes.x;
				int y = indexes.y;
				Vector3 nrm = (newNormals[x] + newNormals[y]) / 2;
				newNormals[x] = newNormals[y] = nrm;
			}

			foreach (BarycentricCoordinates coords in projectedVertices)
			{
				newNormals[coords.index] = (newNormals[coords.i1] * coords.w1 + newNormals[coords.i2] * coords.w2 + newNormals[coords.i3] * coords.w3);
				//newNormals[coords.index] = Vector3.up;  
			}

			normals = new Queue<Vector3>(newNormals);
		}

		void AddVertex(Vector3 position, Vector3 normal, Vector2 uv, Color color)
		{
			verts.Add(position);
			normals.Enqueue(normal);
			uvs.Enqueue(uv);
			colors.Enqueue(color);
		}

		public void AddTriangle(Queue<int> tris,  int a, int b, int c)
		{
			tris.Enqueue(a);
			tris.Enqueue(b);
			tris.Enqueue(c);
		}

		public void AddTriangleToMeshes(int a, int b, int c, int submesh)
		{
			triangles.Enqueue(a);
			triangles.Enqueue(b);
			triangles.Enqueue(c);

			var mesh = submeshes[submesh];
			mesh.Enqueue(a);
			mesh.Enqueue(b);
			mesh.Enqueue(c);
		}
	}

	public struct Vector2Int
	{
		public int x;
		public int y;

		public Vector2Int(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

	}

	public struct BarycentricCoordinates
	{
		public int index; // the index of the vertex/normal that is described by the coordinates

		public int i1; // the index of the first vertex of the triangle
		public int i2; // the index of the second vertex of the triangle
		public int i3; // the index of the third vertex of the triangle

		public float w1; // the weight of the first vertex
		public float w2; // the weight of the second vertex
		public float w3; // the weight of the third vertex

		public BarycentricCoordinates(int index, int i1, int i2, int i3, Vector3 coordinates)
		{
			this.index = index;

			this.i1 = i1;
			this.i2 = i2;
			this.i3 = i3;

			w1 = coordinates.x;
			w2 = coordinates.y;
			w3 = coordinates.z;
		}
	}
}
