using System.ComponentModel.DataAnnotations;

namespace HoPoSim.Data.Domain
{
	public class DbVersion
	{
		[Key]
		public int Id { get; set; }

		public int Version { get; set; }
	}
}
