using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;

namespace HoPoSim.IPC.DAO
{
	public class SimulationData
	{
		public int Id { get; set; }

		public string Name { get; set; }

		[JsonProperty(Required = Required.Always)]
		public PolterConfiguration Poltermaße { get; set; }

		[JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Include)]
		public HolzConfiguration Holz { get; set; } = new HolzConfiguration();

		[JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Include)]
		public BaumartParametrization Baumart { get; set; } = new BaumartParametrization();

		[JsonProperty(Required = Required.Always)]
		public IEnumerable<Stamm> Stämme { get; set; }

		[JsonProperty(Required = Required.Always)]
		public float WoodDensity { get; set; }
		[JsonProperty(Required = Required.Always)]
		public float WoodFriction { get; set; }
	}

	public class Stamm
	{
		[JsonProperty(Required = Required.Always)]
		public string StammId { get; set; }
		[JsonProperty(Required = Required.Always)]
		public float Länge { get; set; }
		[JsonProperty(Required = Required.Always)]
		public int D_Stirn_mR { get; set; }
		[JsonProperty(Required = Required.Always)]
		public int D_Mitte_mR { get; set; }
		[JsonProperty(Required = Required.Always)]
		public int D_Zopf_mR { get; set; }
		[JsonProperty(Required = Required.Always)]
		public int D_Stirn_oR { get; set; }
		[JsonProperty(Required = Required.Always)]
		public int D_Mitte_oR { get; set; }
		[JsonProperty(Required = Required.Always)]
		public int D_Zopf_oR { get; set; }
		[JsonProperty(Required = Required.Always)]
		public int Abholzigkeit { get; set; }
		[JsonProperty(Required = Required.Always)]
		public int Krümmung { get; set; }
		[JsonProperty(Required = Required.Always)]
		public int Rindenstärke { get; set; }
		[JsonProperty(Required = Required.Always)]
		public float Ovalität { get; set; }
		[JsonProperty(Required = Required.Default)]
		public int Stammfußhöhe { get; set; }
		[JsonProperty(Required = Required.Default)]
		public bool HasBranchStubs { get; set; }
	}

	public class Snapshot
	{
		[JsonProperty(Required = Required.Always)]
		public IEnumerable<StammPosition> Positions { get; set; } = new List<StammPosition>();
	}

	public class StammPosition
	{
		[JsonProperty(Required = Required.Always)]
		public string Id { get; set; }
		[JsonProperty(Required = Required.Always)]
		public Position Pos { get; set; }
		[JsonProperty(Required = Required.Always)]
		public Rotation Rot { get; set; }
	}

	public class Position
	{
		[JsonProperty(Required = Required.Always)]
		public float X { get; set; }
		[JsonProperty(Required = Required.Always)]
		public float Y { get; set; }
		[JsonProperty(Required = Required.Always)]
		public float Z { get; set; }
	}

	public class Rotation : Position
	{
		[JsonProperty(Required = Required.Always)]
		public float W { get; set; }
	}

	public class PolterConfiguration
	{
		[JsonProperty(Required = Required.Always)]
		public float MinimumPolterlänge { get; set; }
		[JsonProperty(Required = Required.Always)]
		public float Polterbreite { get; set; }
		public float? Poltertiefe { get; set; }
		[JsonProperty(Required = Required.Always)]
		public bool Polterunterlage { get; set; }
		[JsonProperty(Required = Required.Always)]
		public float Steigungswinkel { get; set; }
		[JsonProperty(Required = Required.Always)]
		public float Seitenspiegelung { get; set; }
		[JsonProperty(Required = Required.Always)]
		public bool Zufallsspiegelung { get; set; }
		[JsonProperty(Required = Required.Default)]
		public float ActualPolterlänge { get; set; }
		[JsonProperty(Required = Required.Default)]
		public float Polterunterlagehöhe { get; set; }
		[JsonProperty(Required = Required.Default)]
		public int Polterunterlagestammanzahl { get; set; }
		[JsonProperty(Required = Required.Default)]
		public int CrossTrunksProportion { get; set; }
		[JsonProperty(Required = Required.Default)]
		public int CrossTrunksMinimumAngle { get; set; }
		[JsonProperty(Required = Required.Default)]
		public int CrossTrunksMaximumAngle { get; set; }
	}

	public class HolzConfiguration
	{
		[JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Include)]
		[DefaultValue(0)]
		public int Rindenbeschädigungen { get; set; }

		[JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Include)]
		[DefaultValue(50)]
		public int Krümmungsvarianten { get; set; }
	}

	public class BaumartParametrization
	{
		public BaumartParametrization()
		{
			Name = "Undefiniert";
		}

		[JsonProperty(Required = Required.Always)]
		public string Name { get; set; }

		[JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Include)]
		public float MinNoiseStrength { get; set; } = 1.0f;// How much minimum noise displacement affects the geometry of the trunk.

		[JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Include)]
		public float MaxNoiseStrength { get; set; } = 1.0f;// How much maximum noise displacement affects the geometry of the trunk.

		[JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Include)]
		public float MinNoiseSize { get; set; } = 1.0f;// How large is the min noise affecting the geometry of the trunk.

		[JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Include)]
		public float MaxNoiseSize { get; set; } = 1.0f;// How large is the max noise affecting the geometry of the trunk.

		[JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Include)]
		public bool IncludeRoots { get; set; } // Wheteher otr not the roots should be modelled

		[JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Include)]
		public int MinRootFlareNumber { get; set; } // Minimum number of flares near the ground

		[JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Include)]
		public int MaxRootFlareNumber { get; set; } // Maximum number of flares near the ground

		[JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Include)]
		public float MinRootRadiusMultiplier { get; set; } = 1.0f;// The minimum radius multiplier of the trunk on the ground.

		[JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Include)]
		public float MaxRootRadiusMultiplier { get; set; } = 1.0f;// The maximum radius multiplier of the trunk on the ground.

		[JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Include)]
		public bool IncludeBranches { get; set; } // Wheteher otr not the branches should be modelled

		[JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Include)]
		public int BranchStubTrunkProportion { get; set; } // The proportion (in %) of trunks that have to be euipped with branch stubs.

		[JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Include)]
		public int BranchStubMinLength { get; set; } // The minimum length (in mm) of a branch stub.

		[JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Include)]
		public int BranchStubMaxLength { get; set; } // The maximum length (in mm) of a branch stub.

		[JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Include)]
		public float BranchStubMinHeight { get; set; } = 1.0f;// The minimum height (in % of the total trunk length) of a branch stub on the trunk.

		[JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Include)]
		public float BranchStubMaxHeight { get; set; } = 1.0f;// The maximum height (in % of the total trunk length) of a branch stub on the trunk.

		[JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Include)]
		public float BranchStubAverageAngle { get; set; } = 1.0f;// The average angle (factor between 0 and 1) made by the branch stubs with respect to the trunk.

		[JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Include)]
		public float BranchStubRadiusMultiplier { get; set; } = 1.0f;// The relative radius of the branch stubs with respect to the trunk's radius.

		[JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Include)]
		public float BranchStubNumberPerMeter { get; set; } = 1.0f; // The average number of branch stub per meter.
	}
}
