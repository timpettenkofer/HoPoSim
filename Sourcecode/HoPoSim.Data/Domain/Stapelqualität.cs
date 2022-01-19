
using System.ComponentModel.DataAnnotations;

namespace HoPoSim.Data.Domain
{
	public class Stapelqualität : BaseEntity
	{
		public int Level { get; set; }

		public int CrossTrunksProportion { get; set; }

		public int CrossTrunksMinimumAngle { get; set; }

		public int CrossTrunksMaximumAngle { get; set; }

		[DataType(DataType.MultilineText)]
		public string Bemerkungen { get; set; }
	}
}
