using HoPoSim.Data.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace HoPoSim.Presentation.Filter
{
    public interface IBooleanFilterDescription { }

    public class BooleanFilterDescription<T> : MultiSelectionFilterDescription<T>, IBooleanFilterDescription where T : class, IHaveNameProperty
    {
        public BooleanFilterDescription(string displayName, string propertyName)
            : base(displayName, propertyName, null)
        { }

        public override bool IsMatch(object candidate)
        {
            var entityValue = GetEntityValue(candidate);
            var value = entityValue is bool ? (bool)entityValue : entityValue != null;
            return SelectedItems.Keys.Contains(value);
        }

        public override bool IsDefaultValue
        {
            get
            {
                return SelectedItems == null || SelectedItems.Count() == BooleanValues.Count();
            }
        }

        public Dictionary<object, string> BooleanValues
        {
            get { return _booleanValues; }
            set
            {
                _booleanValues = value;
                RaisePropertyChanged(nameof(BooleanValues));
            }
        }

        private static Dictionary<object, string> _booleanValues =
            new Dictionary<object, string>
            {
                { true, "Ja" },
                { false, "Nein"}
            };
    }
}
