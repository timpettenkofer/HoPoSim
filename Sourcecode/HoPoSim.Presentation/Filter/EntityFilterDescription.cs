using HoPoSim.Data.Interfaces;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HoPoSim.Presentation.Filter
{
    public interface IEntityFilterDescription : IFilterDescription { }

    public class EntityFilterDescription<T, E> : MultiSelectionFilterDescription<T>, IEntityFilterDescription
        where T : class, IHaveNameProperty
        where E: IHaveIdProperty, IHaveDisplayNameProperty
    {
        public EntityFilterDescription(string displayName, string propertyName, Func<IEnumerable<E>> getLookup, Func<object, object> getValue = null)
            : base(displayName, propertyName, null)
        {
            _getLookup = getLookup;
            _getValue = getValue;
            RefreshLookupCommand = new DelegateCommand(_refreshLookup);
        }

        Func<IEnumerable<E>> _getLookup;

        Func<object, object> _getValue;

        protected override object GetEntityValue(object candidate)
        {
            return _getValue != null ? _getValue(candidate) : base.GetEntityValue(candidate);
        }

        public DelegateCommand RefreshLookupCommand { get; }

        public void _refreshLookup()
        {
			RaisePropertyChanged(nameof(Lookup));
        }

        public override bool IsDefaultValue
        {
            get
            {
                return SelectedItems == null || SelectedItems.Count() == _Lookup.Count();
            }
        }


        public Dictionary<object, string> Lookup
        {
            get
            {
                var l = _getLookup().ToList();
                _Lookup = _getLookup().ToDictionary(i=> (object)i.Id, i => i.DisplayName);
                return _Lookup;
            }
        }
        Dictionary<object, string> _Lookup;
    }
}
