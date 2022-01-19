using System.ComponentModel.DataAnnotations;

namespace HoPoSim.Data.Validation
{
	public class IsNumberAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (value != null)
			{
				int val;
				var isInteger = int.TryParse(value.ToString(), out val);

				if (!isInteger)
					return new ValidationResult(ErrorMessageString);
			}
			return ValidationResult.Success;
		}
	}
}
