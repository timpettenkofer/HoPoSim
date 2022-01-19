using HoPoSim.Data.Interfaces;
using HoPoSim.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HoPoSim.Presentation.Filter
{
    public interface IEnumFilterDescription : IFilterDescription
    {
        Type TargetType { get; }
    }

    public class EnumFilterDescription<T> : MultiSelectionFilterDescription<T>, IEnumFilterDescription
        where T : class, IHaveNameProperty
    {
        public EnumFilterDescription(string displayName, string propertyName, Type targetType)
            : base(displayName, propertyName, null)
        {
            TargetType = targetType;
            EnumValues = Enum.GetValues(TargetType).Cast<object>().ToDictionary(x => x, x => ((Enum)x).GetEnumDescription());
        }

        public  override bool IsDefaultValue
        {
            get
            {
                return SelectedItems == null || SelectedItems.Count() == EnumValues.Count();
            }
        }

        public Dictionary<object, string> EnumValues
        {
            get { return _enumValues; }
            set
            {
                _enumValues = value;
				RaisePropertyChanged(nameof(EnumValues));
            }
        }

        private Dictionary<object, string> _enumValues;

        public Type TargetType { get; }
    }
}
