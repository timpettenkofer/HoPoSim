#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace MTrunk
{
	[System.Serializable]
	public class TrunkParameters
	{
		#region Trunk parameters

		public float RadiusMultiplier = .3f; // The radius of the trunk.
		public AnimationCurve Radius; // The variation of the trunk radius with its length.
		public float Taper = 0.01f;
		public float Ovality = 1.0f;
		public float BendingMultiplier = 0.05f; // The maximal bending of the trunk.
		public AnimationCurve[] BendingShapes; // The variation of the trunk bending with its length. 
		public BendingShape BendingShape; // The variation of the trunk bending with its length.
		public float Length = 5; // The length of the generated trunk.
		public float Randomness = 0f; // How irregular the trunk looks.
		public float OriginAttraction = 0f; // How much the tree is drawn to its original axis. Prevents the trunk from diverging too much.
		public float Resolution = 1.5f; // The amount of points per unit of length.
		public AnimationCurve RootShape; // The evolution of the radius of the trunk near the ground.
		public float RootRadius = 0.0f; // The radius of the trunk on the ground.
		public float RootHeight = 0.0f; // The height to which the root goes.
		public float RootResolutionMultiplier = 1f; // How much more resolution to add to the trunk near the ground.
		public int FlareNumber = 0; // Number of flares near the ground.
		public float SpinAmount = 0.0f; // How much the trunk is twisted.
		public float DisplacementStrength = 0f; // How much noise affects the geometry of the trunk.
		public float DisplacementSize = 0.0f; // How large is the noise affecting the geometry of the trunk.
		public float HeightOffset = 0.0f; // How much the trunk goes inside the ground. Helps when a tree is placed on an uneven ground.
		public float RadialResolution = 1.5f;
		public float BarkThickness = 0.0f;

		public int BranchNumber = 0;
		public float BranchAngle = .85f;
		public float BranchRadius = .6f;
		public float BranchStart = .2f;
		public float BranchEnd = .8f;

		public int BranchMinLength = 5; // min branch length in mm
		public int BranchMaxLength = 30;

		#endregion

		public TrunkParameters()
		{
			Keyframe[] keys = new Keyframe[2] { new Keyframe(0f, 1f, 0f, 0f), new Keyframe(1f, 1f, 0f, 0f) };
			Radius = new AnimationCurve(keys);
			Keyframe[] rootKeys = new Keyframe[2] { new Keyframe(0f, 1f, 0f, -1.2f), new Keyframe(1f, 0f, -0.4f, 0f) };
			RootShape = new AnimationCurve(rootKeys);

			Keyframe[] endCurvatureBendingKeys = new Keyframe[4] { new Keyframe(0f, 0f, 0.0f, 1.8f), new Keyframe(0.5f, 0.9f, 1.8f, 1.8f), new Keyframe(0.6f, 1f, 0f, 0f), new Keyframe(1f, 0f, -7.5f, 0f) }; // bended after first half
			Keyframe[] bananaBendingKeys = new Keyframe[3] { new Keyframe(0f, 0f, 0f, 5.0f), new Keyframe(0.5f, 1f, 0f, 0f), new Keyframe(1f, 0f, -5f, 0f) }; //banana

			BendingShapes = new AnimationCurve[] { new AnimationCurve(endCurvatureBendingKeys), new AnimationCurve(bananaBendingKeys) };
			BendingShape = BendingShape.EndCurvature;
		}

		public void Apply(Trunk tree)
		{
			//Random.InitState(seed);
			tree.Create(this);
			tree.AddBranches(this);
		}
	}

	//public enum FunctionType {Trunk}
	public enum BendingShape { EndCurvature = 0, Banana}

}