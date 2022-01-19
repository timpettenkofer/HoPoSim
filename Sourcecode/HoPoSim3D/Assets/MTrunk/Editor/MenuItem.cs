using UnityEngine;
using UnityEditor;

public class MenuItems
{
	[MenuItem("GameObject/MTrunk/Create Trunk")]
	private static void NewMenuOption()
	{
		GameObject trunk = new GameObject("trunk");
		TrunkComponent mtrunk = trunk.AddComponent<TrunkComponent>();
		mtrunk.SetParameters(null);
		mtrunk.Generate();
		Selection.activeGameObject = trunk;
	}
}
