using HoPoSim.Data.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HoPoSim.Data.Domain
{
	public class SimulationData : BaseEntity
	{
		[Required(AllowEmptyStrings = false, ErrorMessageResourceName = "Entity_RequiredFieldFailed", ErrorMessageResourceType = typeof(Properties.Resources))]
		public string Name { get; set; }

		[DataType(DataType.MultilineText)]
		public string Bemerkungen { get; set; }

		public string SourceFile { get; set; }

		public Stammdaten Stammliste { get; set; }

		public float Polterlänge { get; set; }

		public float? Poltertiefe { get; set; }

		public bool Polterunterlage { get; set; }

		public float Steigungswinkel { get; set; }

		public float Seitenspiegelung { get; set; }

		public bool Zufallsspiegelung { get; set; }

		public float WoodDensity { get; set; }

		public float WoodFriction { get; set; }

		public int Rindenbeschädigungen { get; set; }

		public int Krümmungsvarianten { get; set; }

		[ForeignKey("BaumartId")]
		public virtual BaumartParametrization Baumart { get; set; }
		public int BaumartId { get; set; }

		[ForeignKey("StapelqualitätId")]
		public virtual Stapelqualität Stapelqualität { get; set; }
		public int StapelqualitätId { get; set; }


		public bool HasStammData()
		{
			return Stammliste?.DataTable != null;
		}

		public bool IsValidInput()
		{
			return HasStammData();
		}
	}
}

