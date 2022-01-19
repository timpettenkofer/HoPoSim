using System.ComponentModel.DataAnnotations;

namespace HoPoSim.Presentation.Validation
{
    public class IsNotNullForeignKey : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var id = value as int?;
            if (!id.HasValue)
                return new ValidationResult("Dieses Feld ist ein Pflichtfeld und muss gesetzt werden.");
            return ValidationResult.Success;
        }
    }
}
