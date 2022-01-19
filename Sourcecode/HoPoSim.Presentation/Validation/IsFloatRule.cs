using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Windows.Controls;
using ValidationResult = System.Windows.Controls.ValidationResult;

namespace HoPoSim.Presentation.Validation
{
    public class IsFloatRule : System.Windows.Controls.ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value != null)
            {
                float val;
                var isFloat = float.TryParse(value.ToString(), out val);

                if (!isFloat)
                {
                    return new ValidationResult(false, Properties.Resources.IsFloatRuleValidationFailed);
                }
            }

            return new ValidationResult(true, null);
        }
    }


}
