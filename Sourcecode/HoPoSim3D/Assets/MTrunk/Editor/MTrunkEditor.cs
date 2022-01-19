using UnityEngine;
using UnityEditor;
using MTrunk;
using System;

[CanEditMultipleObjects]
[CustomEditor(typeof(TrunkComponent))]
public class MTrunkEditor: Editor
{
	TrunkComponent trunk;
	private string[] tabNames = { "Parameters", "Quality", "Save as prefab" };
	private int tabIndex = 0; // Used to navigate beetween tabs 
	private bool UndoDirty = false;

	private void OnEnable()
	{
		trunk = (TrunkComponent)target;
		if (trunk.trunk == null)
		{
			UpdateTrunk();
		}
		Undo.undoRedoPerformed += UndoCallback;
	}

	public override void OnInspectorGUI()
	{
		if (UndoDirty)
		{
			UndoDirty = false;
			UpdateTrunk();
		}

		if (IsMultiSelection()) // Editor to display when multiple trees are selected
		{
			DisplayMultiObjectsEditting();
			return; // Not drawing the rest when multpile trees are selected
		}
		
		if (trunk.ModelerVersion != MTrunk.Version.version)
		{
			EditorGUILayout.HelpBox("Warning, this trunk was made with a previous version of MTrunk, you need to upgrade the tree. Upgrading it may result in unwanted changes.", MessageType.Warning);
			if (GUILayout.Button("Upgrade tree"))
			{
				trunk.ModelerVersion = MTrunk.Version.version;
			}
			return;
		}


		tabIndex = GUILayout.SelectionGrid(tabIndex, tabNames, tabNames.Length);

		if (tabIndex == 1) // Quality tab
		{
			DisplayQualityTab();
		}

		else
		if (tabIndex == 2) // Save as prefab tab
		{
			DisplaySaveTab();
		}

		else
		{
			DisplayParametersTab();    
		}

		EditorGUILayout.LabelField("polycount: " + trunk.polycount.ToString(), EditorStyles.boldLabel);
	}

	private void DisplayQualityTab()
	{
		EditorGUILayout.BeginVertical(EditorStyles.helpBox);
		EditorGUI.BeginChangeCheck();
		TrunkParameters parameters = trunk.trunkParameters;
		parameters.RadialResolution = EditorGUILayout.FloatField("Radial Resolution", parameters.RadialResolution);
		parameters.RadialResolution= Mathf.Max(0, parameters.RadialResolution);
		trunk.simplifyAngleThreshold = EditorGUILayout.Slider("Simplify angle", trunk.simplifyAngleThreshold, 0, 90);
		trunk.simplifyRadiusThreshold = EditorGUILayout.Slider("Simplify radius", trunk.simplifyRadiusThreshold, 0, .9f);

		if (EditorGUI.EndChangeCheck())
			UpdateTrunk();
		EditorGUILayout.EndVertical();
	}

	private void DisplaySaveTab()
	{
		EditorGUILayout.BeginVertical(EditorStyles.helpBox);

		EditorGUILayout.BeginHorizontal();
		trunk.saveTreeFolder = EditorGUILayout.TextField("Save Folder", trunk.saveTreeFolder);
		if (GUILayout.Button("Find folder"))
		{
			string path = EditorUtility.OpenFolderPanel("save tree location", "Assets", "Assets");
			path = "Assets" + path.Substring(Application.dataPath.Length);
			trunk.saveTreeFolder = path;
			AssetDatabase.Refresh();
		}

		EditorGUILayout.EndHorizontal();
		trunk.gameObject.name = EditorGUILayout.TextField("name", trunk.gameObject.name);

		//if (GUILayout.Button("Save as Prefab"))
		//	trunk.SaveAsPrefab();
		EditorGUILayout.EndVertical();
	}

	private void DisplayMultiObjectsEditting()
	{
		EditorGUILayout.BeginVertical(EditorStyles.helpBox);
		EditorGUILayout.BeginHorizontal();
		string saveTreeFolder = EditorGUILayout.TextField("Save Folder", trunk.saveTreeFolder);
		if (GUILayout.Button("Find folder"))
		{
			string path = EditorUtility.OpenFolderPanel("save tree location", "Assets", "Assets");
			if (path.Length > 0)
			{
				path = "Assets" + path.Substring(Application.dataPath.Length);
				saveTreeFolder = path;
				foreach (TrunkComponent t in Array.ConvertAll(targets, item => (TrunkComponent)item))
				{
					t.saveTreeFolder = saveTreeFolder;
				}
				AssetDatabase.Refresh();
			}            
		}
		EditorGUILayout.EndHorizontal();
		


		//if (GUILayout.Button("Save all as Prefabs"))
		//{
		//	saveTreeFolder = trunk.saveTreeFolder;
		//	foreach (TrunkComponent t in Array.ConvertAll(targets, item => (TrunkComponent)item))
		//	{
		//		//t.saveTreeFolder = saveTreeFolder;
		//		t.SaveAsPrefab(groupedSave:true);
		//	}
		//	UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(saveTreeFolder, typeof(UnityEngine.Object));
		//	// Select the object in the project folder
		//	Selection.activeObject = obj;
		//	// Also flash the folder yellow to highlight it
		//	EditorGUIUtility.PingObject(obj);
		//}
		EditorGUILayout.EndVertical();
	}

	private void DisplayParametersTab()
	{
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.BeginVertical(EditorStyles.helpBox);
		TrunkParameters f = trunk.trunkParameters;

		f.Length = EditorGUILayout.FloatField("Length", f.Length);
		f.Length = Mathf.Max(0.01f, f.Length);
		f.RadiusMultiplier = EditorGUILayout.FloatField("Radius", f.RadiusMultiplier);
		f.RadiusMultiplier = Mathf.Max(0.0001f, f.RadiusMultiplier);
		f.Taper = EditorGUILayout.FloatField("Taper", f.Taper);
		f.Taper = Mathf.Max(0.0f, f.Taper);
		f.Radius = EditorGUILayout.CurveField("Radius Shape", f.Radius);
		f.Ovality = EditorGUILayout.FloatField("Ovality", f.Ovality);
		f.Ovality = Mathf.Max(0.0001f, f.Ovality);

		f.BendingMultiplier = EditorGUILayout.FloatField("Bending", f.BendingMultiplier);
		f.BendingShape = (BendingShape)EditorGUILayout.EnumPopup("Bending Shape", f.BendingShape);

		f.Resolution = EditorGUILayout.FloatField("Resolution", f.Resolution);
		f.Resolution = Mathf.Max(.01f, f.Resolution);
		f.OriginAttraction = EditorGUILayout.Slider("Axis attraction", f.OriginAttraction, 0, 1);
			
		f.Randomness = EditorGUILayout.Slider("Randomness", f.Randomness, 0f, 0.5f);
		f.DisplacementStrength = EditorGUILayout.FloatField("Displacement strength", f.DisplacementStrength);
		f.DisplacementSize = EditorGUILayout.FloatField("Displacement size", f.DisplacementSize);
		f.SpinAmount = EditorGUILayout.FloatField("Spin amount", f.SpinAmount);
		f.HeightOffset = EditorGUILayout.FloatField("Height Offset", f.HeightOffset);

		EditorGUILayout.BeginVertical(EditorStyles.helpBox);
		EditorGUILayout.LabelField("Root");
		f.RootShape = EditorGUILayout.CurveField("Root shape", f.RootShape);
		f.RootHeight = EditorGUILayout.FloatField("Height", f.RootHeight);
		f.RootRadius = EditorGUILayout.Slider("Root Radius", f.RootRadius, 0, 2);
		f.RootResolutionMultiplier = EditorGUILayout.FloatField("Additional Resolution", f.RootResolutionMultiplier);
		f.RootResolutionMultiplier = Mathf.Max(1, f.RootResolutionMultiplier);
		f.FlareNumber = EditorGUILayout.IntSlider("Flare Number", f.FlareNumber, 0, 10);
		EditorGUILayout.EndVertical();


		EditorGUILayout.BeginVertical(EditorStyles.helpBox);
		EditorGUILayout.LabelField("Branches");
		f.BranchNumber = EditorGUILayout.IntField("Branch Number", f.BranchNumber);

		//EditorGUILayout.BeginHorizontal();
		//f.BranchLength = EditorGUILayout.FloatField("Branch Length", f.BranchLength);
		//f.BranchLengthCurve = EditorGUILayout.CurveField("Branch Length Curve", f.BranchLengthCurve);
		//EditorGUILayout.EndHorizontal();

		//f.BranchResolution = EditorGUILayout.FloatField("Branch Resolution", f.BranchResolution);
		//f.BranchRandomness = EditorGUILayout.Slider("Branch Randomness", f.BranchRandomness, 0, 2);
		f.BranchRadius = EditorGUILayout.Slider("Branch Radius", f.BranchRadius, 0.001f, 1);
		//f.BranchShape = EditorGUILayout.CurveField("Branch Shape", f.BranchShape);

		//EditorGUILayout.BeginHorizontal();
		//f.BranchSplitProbability = EditorGUILayout.Slider("Branch Split Probability", f.BranchSplitProbability, 0, 1);
		//f.BranchSplitProbabilityCurve = EditorGUILayout.CurveField("Branch Split Probability Curve", f.BranchSplitProbabilityCurve);
		//EditorGUILayout.EndHorizontal();

		f.BranchAngle = EditorGUILayout.Slider("Branch Angle", f.BranchAngle, 0, 1);
		//f.BranchUpAttraction = EditorGUILayout.FloatField("Branch Up Attraction", f.BranchUpAttraction);
		//f.BranchGravityStrength = EditorGUILayout.FloatField("Branch Gravity Strength", f.BranchGravityStrength);
		f.BranchStart = EditorGUILayout.Slider("Branch Start", f.BranchStart, 0, 1);
		f.BranchEnd = EditorGUILayout.Slider("Branch End", f.BranchEnd, 0, 1);

		f.BranchMinLength = EditorGUILayout.IntField("Branch Min Length", f.BranchMinLength);
		f.BranchMaxLength = EditorGUILayout.IntField("Branch Max Length", f.BranchMaxLength);

		EditorGUILayout.EndVertical();


		EditorGUILayout.EndVertical();
		if (EditorGUI.EndChangeCheck())
		{
			UpdateTrunk();
		}
	}

	private void UpdateTrunk()
	{
		trunk.Generate();
	}

	private bool IsMultiSelection()
	{
		return targets.Length > 1;
	}
	
	private void UndoCallback()
	{
		UndoDirty = true;
	}
}


