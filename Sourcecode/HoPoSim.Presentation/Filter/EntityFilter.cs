using HoPoSim.Data.Interfaces;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HoPoSim.Presentation.Filter
{
    public class EntityFilter<T> : BindableBase where T: class, IHaveNameProperty
    {
        public EntityFilter()
        {
            GetSearchableFields = e => e.Name;
            ClearFilters();
        }

        public Func<T, string> GetSearchableFields { get; set; }

		public virtual string ToDisplayString(bool addDefaultActiveFilter = false)
		{
			return addDefaultActiveFilter ? ToString() : string.Empty;
		}

		public override string ToString()
        {
            var filters = new[] { "Aktive", "Archivierte" };
            return string.Join(SeparatorString, filters.Select(f => f + " = " + this[f]));
        }

        public string SeparatorString {  get { return _separatorString; } }
        public static string _separatorString = " \u25CF ";

        public string ConcatFilterString(string prefix, string str)
        {
            if (string.IsNullOrEmpty(prefix))
                return str;
            if (string.IsNullOrEmpty(str))
                return prefix;
            return $"{prefix}{SeparatorString}{str}";

        }

        public bool Filter(object item)
        {
            var entity = item as T;
            return ApplyFilter(entity);
        }

        public bool ApplyFilter(T entity)
        {
            if (ApplyFilterFunc != null)
                return ApplyFilterFunc(entity);
            return Match(entity);
        }

        public Func<T, bool> ApplyFilterFunc
        {
            get; set;
        }

        public virtual bool Match(T entity)
        {
            return MatchName(entity) && MatchFilter(entity) && MatchStatus(entity);
        }

        protected virtual bool MatchFilter(T entity)
        {
            return true;
        }

        public void ClearFilters()
        {
            this["Aktive"] = true;
            this["Archivierte"] = false;
        }

        public bool CanClearFilters()
        {
            return (bool)this["Aktive"] != true || (bool)this["Archivierte"] != false;
        }

        public virtual bool CanResetFilters()
        {
            return false;
        }
        public virtual void ResetFilters()
        { }

        protected virtual bool MatchName(T entity)
        {
            if (string.IsNullOrEmpty(SearchString))
                return true;

            var filters = SearchString
                .Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries)
                .Select(v => v.ToLower());

            var values = GetSearchableFields(entity)
                .Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries)
                .Select(v => v.ToLower());

            return filters.All(f => values.Any(v => v.Contains(f)));
        }

		// TODO extract activable
		protected bool MatchStatus(T entity)
		{
			return !(entity is IHaveActiveProperty) ||  (entity as IHaveActiveProperty).Active ? (bool)this["Aktive"] : (bool)this["Archivierte"];
		}


		public string SearchString
        {
            get { return _searchString; }
            set
            {
                _searchString = value;
				RaisePropertyChanged(nameof(SearchString));
                FilterChanged(this, EventArgs.Empty);
            }
        }
        private string _searchString;
         
        public object this[string propertyName]
        {
            get
            {
                object value;
                if (_filters.TryGetValue(propertyName, out value))
                    return value;
                return null;
            }
            set
            {
                if (value != null)
                {
                    _filters[propertyName] = value;
                    FilterChanged(this, EventArgs.Empty);
                    //RaisePropertyChanged("Item[" + propertyName + "]");
                }
            }
        }

        public event EventHandler FilterChanged = delegate { };

        protected void OnFilterChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IFilterDescription.SelectedValue))
                FilterChanged(this, EventArgs.Empty);
        }

        Dictionary<string, object> _filters = new Dictionary<string, object>();
    }
}
