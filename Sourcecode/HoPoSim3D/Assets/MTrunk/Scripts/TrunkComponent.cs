using UnityEngine;
using MTrunk;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TrunkComponent : MonoBehaviour {

	public TrunkParameters trunkParameters;
	public Trunk trunk;
	public float simplifyAngleThreshold = 0;
	public float simplifyRadiusThreshold = 0f;
	public string saveTreeFolder = "Assets";
	public int polycount = 0;
	public string ModelerVersion;

	void InitializeTrunk()
	{
		ModelerVersion = Version.version;
		trunk = new Trunk(transform);
	}

	public void SetParameters(TrunkParameters t)
	{
		trunkParameters = t;
		if (trunkParameters == null)
			trunkParameters = new TrunkParameters();
	}

	private void Build()
	{
		if (trunk == null)
			InitializeTrunk();
		SetParameters(trunkParameters);
		trunkParameters.Apply(trunk);
	}

	public void Generate()
	{
		//if (trunkParameters != null && ModelerVersion != Version.version)
		//	return null;
		trunk = null;
		Build();
		trunk.Simplify(simplifyAngleThreshold, simplifyRadiusThreshold);
		Mesh mesh = CreateMesh(trunkParameters);
		GetComponent<MeshFilter>().mesh = mesh;
		polycount = mesh.triangles.Length / 3;
		CreateCompoundColliders(trunk.colliders);
		UpdateMaterials();
	}

	public Mesh GenerateMesh(TrunkParameters parameters)
	{
		if (trunk == null)
			InitializeTrunk();
		parameters.Apply(trunk);
		return CreateMesh(parameters);
	}

	private Mesh CreateMesh(TrunkParameters parameters)
	{
		Mesh mesh = new Mesh();
		if (parameters != null)
			trunk.GenerateMeshData(parameters);
		mesh.vertices = trunk.verts;
		mesh.normals = trunk.normals;
		mesh.uv = trunk.uvs;
		Color[] colors = trunk.colors;
		mesh.triangles = trunk.triangles;
		mesh.colors = colors;

		mesh.subMeshCount = trunk.subMeshCount;
		for (var i = 0; i < trunk.subMeshCount; ++i)
			mesh.SetTriangles(trunk.submeshes[i], i);

		mesh.RecalculateNormals();
		return mesh;
	}

	void CreateCompoundColliders(Mesh[] meshes)
	{
		var colliders = gameObject.GetComponentsInChildren<MeshCollider>();
		foreach (var collider in colliders)
		{
#if UNITY_EDITOR
			DestroyImmediate(collider.gameObject);
#else
			Destroy(collider.gameObject);
#endif
		}

		for (int i = 0; i < meshes.Length; ++i)
		{
			var obj = new GameObject($"collider_{i}");
			var collider = obj.AddComponent<MeshCollider>();
			collider.sharedMesh = meshes[i];
			collider.convex = true;
			collider.enabled = true;
			collider.convex = true;
			collider.cookingOptions = MeshColliderCookingOptions.CookForFasterSimulation |
				MeshColliderCookingOptions.EnableMeshCleaning |
				MeshColliderCookingOptions.WeldColocatedVertices;
			collider.transform.SetPositionAndRotation(gameObject.transform.position, gameObject.transform.rotation);
			obj.transform.parent = gameObject.transform;
		}
	}

	public void UpdateMaterials()
	{
		MeshRenderer renderer = GetComponent<MeshRenderer>();
		{
			switch (renderer.sharedMaterials.Length)
			{
				case 1:
					if (renderer.sharedMaterial == null)
					{
						var mat_section = SectionMaterial;
						var mat_bark = trunkParameters.BarkThickness > 0? BarkMaterial : NoBarkMaterial;
						renderer.sharedMaterials = new Material[] { mat_bark, mat_section };
					}
					break;

				case 2:
					renderer.sharedMaterials = new Material[] { renderer.sharedMaterials[0], renderer.sharedMaterials[1] };
					break;
			}
		}
	}

	private static Material BarkMaterial
	{
		get
		{
			if (_barkMaterial == null)
				_barkMaterial = Resources.Load<Material>("Materials/Pine_C");
			return _barkMaterial;
		}
	}
	private static Material _barkMaterial;

	private static Material SectionMaterial
	{
		get
		{
			if (_sectionMaterial == null)
				_sectionMaterial = Resources.Load<Material>("Materials/Section");
			return _sectionMaterial;
		}
	}
	private static Material _sectionMaterial;

	private static Material NoBarkMaterial
	{
		get
		{
			if (_noBarkMaterial == null)
				_noBarkMaterial = Resources.Load<Material>("Materials/Bark_Damaged_C");
			return _noBarkMaterial;
		}
	}
	private static Material _noBarkMaterial;

#if UNITY_EDITOR
	// TODO check if necessary
	public Material[] SaveMaterials(string folderPath)
	{
		MeshRenderer renderer = GetComponent<MeshRenderer>();
		Material[] materialsCopy = new Material[renderer.sharedMaterials.Length];
		int matIndex = 0;
		foreach (Material mat in renderer.sharedMaterials)
		{
			if (AssetDatabase.GetAssetPath(mat).Length == 0)
			{
				string matName = Path.GetFileName(mat.name);
				string matPath = Path.Combine(folderPath, matName + ".mat");
				Material matCopy = new Material(mat);
				materialsCopy[matIndex] = matCopy;
				AssetDatabase.CreateAsset(matCopy, matPath);
			}
			else
			{
				materialsCopy[matIndex] = mat;
			}
			matIndex++;
		}

		return materialsCopy;
	}
#endif


	//public void SaveAsPrefab(bool groupedSave = false)
	//{
	//	string name = gameObject.name;
	//	string path = saveTreeFolder;
	//	if (string.IsNullOrEmpty(path))
	//		return;

	//	bool replacePrefab = false;

	//	if (!System.IO.Directory.Exists(path))
	//	{
	//		EditorUtility.DisplayDialog("Invalid Path", "The path is not valid, you can chose it with the find folder button", "Ok");
	//		return;
	//	}
	//	if (AssetDatabase.LoadAssetAtPath(path + "/" + name + ".prefab", typeof(GameObject))) // Overriding prefab dialog
	//	{
	//		if (EditorUtility.DisplayDialog("Are you sure?", "The prefab already exists. Do you want to overwrite it?", "Yes", "No"))
	//		{
	//			FileUtil.DeleteFileOrDirectory(Path.Combine(path, name + "_meshes"));
	//			AssetDatabase.Refresh();
	//			replacePrefab = true;
	//		}
	//		else
	//		{
	//			name += "_1";
	//		}
	//	}

	//	Mesh[] meshes = new Mesh[4];
	//	string meshesFolder = AssetDatabase.CreateFolder(path, name + "_meshes");
	//	meshesFolder = AssetDatabase.GUIDToAssetPath(meshesFolder) + Path.DirectorySeparatorChar;
	//	Material[] materials = SaveMaterials(meshesFolder);
	//	GameObject TreeObject = new GameObject(name); // Tree game object

	//	string prefabPath = path + "/" + name + ".prefab";

	//	Object prefab;
	//	if (replacePrefab)
	//	{
	//		Object targetPrefab = AssetDatabase.LoadAssetAtPath(path + "/" + name + ".prefab", typeof(GameObject));
	//		prefab = PrefabUtility.ReplacePrefab(TreeObject, targetPrefab, ReplacePrefabOptions.ConnectToPrefab);
	//	}
	//	else
	//	{
	//		prefab = PrefabUtility.CreatePrefab(prefabPath, TreeObject, ReplacePrefabOptions.ConnectToPrefab);
	//	}
		
	//	AssetDatabase.SaveAssets();
	//	DestroyImmediate(TreeObject);
		
	//	if (!groupedSave)
	//	{
	//		// select newly created prefab in folder
	//		Selection.activeObject = prefab;
	//		// Also flash the folder yellow to highlight it
	//		EditorGUIUtility.PingObject(prefab);
	//		EditorUtility.DisplayDialog("Prefab saved !", "The prefab is saved, you can now delete the tree and use the prefab instead", "Ok");
	//	}

	//}
}


