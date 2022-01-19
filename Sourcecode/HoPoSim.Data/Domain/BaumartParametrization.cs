using System.ComponentModel.DataAnnotations;

namespace HoPoSim.Data.Domain
{
	public class BaumartParametrization : BaseEntity
	{
		[Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Entity_RequiredFieldFailed", ErrorMessageResourceType = typeof(Properties.Resources))]
		public string Name { get; set; }

		public float MinNoiseStrength { get; set; } // How much minimum noise displacement affects the geometry of the trunk.
		public float MaxNoiseStrength { get; set; } // How much maximum noise displacement affects the geometry of the trunk.

		public float MinNoiseSize { get; set; } // How large is the min noise affecting the geometry of the trunk.
		public float MaxNoiseSize { get; set; } // How large is the max noise affecting the geometry of the trunk.

		public bool IncludeRoots { get; set; } // Whether or not the roots should be modelled

		public int MinRootFlareNumber { get; set; } // Minimum number of flares near the ground
		public int MaxRootFlareNumber { get; set; } // Maximum number of flares near the ground

		public float MinRootRadiusMultiplier { get; set; } // The minimum radius multiplier of the trunk on the ground.
		public float MaxRootRadiusMultiplier { get; set; } // The maximum radius multiplier of the trunk on the ground.

		public bool IncludeBranches { get; set; } // Whether or not branches should be modelled

		public int BranchStubTrunkProportion { get; set; } = 0; // The proportion (in %) of trunks that have to be equipped with branch stubs.
		public int BranchStubMinLength { get; set; } = 5; // The minimum length (in mm) of a branch stub.
		public int BranchStubMaxLength { get; set; } = 30; // The maximum length (in mm) of a branch stub.
		public float BranchStubMinHeight { get; set; } = 0.3f; // The minimum height (in % of the total trunk length) of a branch stub on the trunk.
		public float BranchStubMaxHeight { get; set; } = 0.8f; // The maximum height (in % of the total trunk length) of a branch stub on the trunk.
		public float BranchStubAverageAngle { get; set; } = 1f;// The average angle (factor between 0 and 1) made by the branch stubs with respect to the trunk.
		public float BranchStubNumberPerMeter { get; set; } = 0f;// The average number of branch stub per meter.
		public float BranchStubRadiusMultiplier { get; set; } = 0.6f;// The radius multiplier of branch stub.
	}
}
