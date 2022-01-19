using System.Linq;
using UnityEngine;

public class DecorationController : MonoBehaviour
{
	public KeyCode Key_Id;
	public KeyCode Key_Outline;
	public KeyCode Key_Hull;
	public KeyCode Key_Optik;
	public KeyCode Key_Sektion;
	public KeyCode Key_Help;

	public GameObject HelpText;
	public GameObject HelpButton;

	void Start() { Init(); }
	void OnEnable() { Init(); }

	public void Init()
	{
		if (Key_Id == KeyCode.None)
			Key_Id = KeyCode.N;
		if (Key_Outline == KeyCode.None)
			Key_Outline = KeyCode.F;
		if (Key_Optik == KeyCode.None)
			Key_Optik = KeyCode.O;
		if (Key_Hull == KeyCode.None)
			Key_Hull = KeyCode.P;
		if (Key_Sektion == KeyCode.None)
			Key_Sektion = KeyCode.S;
		if (Key_Help == KeyCode.None)
			Key_Help = KeyCode.I;
	}

	void LateUpdate()
	{
		if (Input.GetKeyDown(Key_Id))
		{
			ToggleLabels();
		}

		if (Input.GetKeyDown(Key_Outline))
		{
			ToggleOutlines();
		}

		if (Input.GetKeyDown(Key_Optik))
		{
			ToggleFotooptik();
		}

		if (Input.GetKeyDown(Key_Hull))
		{
			ToggleHull();
		}

		if (Input.GetKeyDown(Key_Sektion))
		{
			ToggleSektion();
		}

		if (Input.GetKeyDown(Key_Help))
		{
			ToggleHelp();
		}
	}

	public static void ToggleHull()
	{
		var hull = GameObject.FindObjectOfType<ConcaveHullOutline>();
		if (hull.IsVisible(Side.BOTH))
			hull.Hide(Side.BOTH);
		else
			hull.Show(Side.BOTH);
	}

	public static void ToggleSektion()
	{
		var sektion = GameObject.FindObjectOfType<Sektionraummaß>();
		if (sektion.IsVisible(Side.BOTH))
			sektion.Hide(Side.BOTH);
		else
			sektion.Show(Side.BOTH, 0.02f, true);
	}

	public static void ToggleFotooptik()
	{
		var sektion = GameObject.FindObjectOfType<Fotooptik>();
		if (sektion.IsVisible(Side.BOTH))
			sektion.Hide(Side.BOTH);
		else
			sektion.Show(Side.BOTH, 0.02f, true);
	}

	public void ShowHelpButton()
	{
		if (HelpButton != null)
			HelpButton.SetActive(true);
	}

	public void ToggleHelp()
	{
		if (HelpText != null)
			HelpText.SetActive(!HelpText.activeSelf);
	}


	public static void ToggleLabels()
	{
		var trunks = Resources.FindObjectsOfTypeAll<TrunkLabel>().ToList();
		trunks.ForEach(t => t.SetVisible(!t.IsVisible()));
	}

	public static void ToggleOutlines()
	{
		var trunks = Resources.FindObjectsOfTypeAll<TrunkOutline>().ToList();
		trunks.ForEach(t => t.SetVisible(!t.IsVisible()));
	}
}
