using System.ComponentModel.DataAnnotations;

namespace HoPoSim.Data.Validation
{
	public class IsValidPercentageAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (value is double)
			{
				var v = (double)value;
				if (v < 0 || v > 100)
					return new ValidationResult("Der Wert muss zwischen between 0 and 100 liegen.");
				return ValidationResult.Success;
			}
			return new ValidationResult("Der Wert muss eine Zahl sein.");
		}
	}
}
