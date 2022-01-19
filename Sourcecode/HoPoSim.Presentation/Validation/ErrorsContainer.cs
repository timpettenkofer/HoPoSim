using System;
using System.Linq;

namespace HoPoSim.Presentation.Validation
{
    public class ErrorsContainer : Prism.Mvvm.ErrorsContainer<string>
    {
        public ErrorsContainer(Action<string> raiseErrorsChanged) : base(raiseErrorsChanged)
        {
        }


        public string FormattedValidationResults
        {
            get
            {
                return  string.Join(Environment.NewLine, validationResults.Select(vr => $"{vr.Key}: {string.Join(Environment.NewLine, vr.Value)}"));
            }
        }
    }
}
