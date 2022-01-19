using HoPoSim.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HoPoSim.Presentation.Filter
{
    public class CustomEntityFilter<T>: EntityFilter<T> where T : class, IHaveNameProperty
    {
        public CustomEntityFilter()
        {
            _filter = CustomUserFilter;
            UserFilterDescriptions = Enumerable.Empty<IFilterDescription>();
        }
        Func<T, bool> _filter = null;

        public void InitializeFilter(IEnumerable<IFilterDescription> entitiesFilterDescriptions)
        {
            foreach (var ufd in UserFilterDescriptions)
                ufd.PropertyChanged -= OnFilterChanged;
            UserFilterDescriptions = entitiesFilterDescriptions;
            foreach (var ufd in UserFilterDescriptions)
                ufd.PropertyChanged += OnFilterChanged;
        }

        public IEnumerable<IFilterDescription> UserFilterDescriptions
        {
            get; private set;
        }

		public override string ToDisplayString(bool addDefaultActiveFilter = false)
		{
			var filterString = string.Join(SeparatorString,
				UserFilterDescriptions.
					Where(fd => !fd.IsDefaultValue).Select(fd => fd.ToString()));
			return addDefaultActiveFilter ?
				ConcatFilterString(filterString, base.ToString()) :
				filterString;
		}

        public override string ToString()
        {
			return ToDisplayString(true);
        }

        public override void ResetFilters()
        {
            foreach (var fd in UserFilterDescriptions)
            {
                fd.SelectedValue = fd.DefaultValue;
            }
        }

        public override bool CanResetFilters()
        {
            return UserFilterDescriptions.Any(fd => !fd.IsDefaultValue);
        }

        private bool CustomUserFilter(T vm)
        {
            return UserFilterDescriptions.All(fd => fd.IsDefaultValue || fd.IsMatch(vm));
        }

        protected override bool MatchFilter(T entity)
        {
            return _filter(entity);
        }
    }
}
