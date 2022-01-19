using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoPoSim.Presentation.Validation
{
    public class IsFloatAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            if (value != null)
            {
                float val;
                var isFloat = float.TryParse(value.ToString(), out val);

                if (!isFloat)
                    return new ValidationResult(ErrorMessageString);
            }
            return ValidationResult.Success;
        }
    }
}
