using HoPoSim.IPC.DAO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace HoPoSim.Presentation.Validation
{
	public class IsValidSerializedSimulationDataAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			var data = value as string;
			if (data == null)
				return new ValidationResult("Value must be a string.");

			IList<string> errors = new List<string>();
			var success = Serializer<SimulationData>.Validate(data, out errors);

			if (!success)
				return new ValidationResult(errors.First());
			return ValidationResult.Success;
		}
	}
}
