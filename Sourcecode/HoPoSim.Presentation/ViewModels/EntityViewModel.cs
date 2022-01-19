using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using HoPoSim.Presentation.Filter;
using System.Globalization;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using HoPoSim.Data.Interfaces;
using HoPoSim.Data.Validation;
using HoPoSim.Presentation.Attributes;
using HoPoSim.Presentation.Helpers;
using HoPoSim.Framework.Interfaces;
using HoPoSim.Framework.Attributes;

namespace HoPoSim.Presentation.ViewModels
{
	public class RangeObservableCollection<T> : ObservableCollection<T>
	{
		private bool _suppressNotification = false;
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (!_suppressNotification)
				base.OnCollectionChanged(e);
		}

		public void AddRange(IEnumerable<T> list)
		{
			if (list == null)
				throw new ArgumentNullException("list");

			_suppressNotification = true;

			foreach (T item in list)
				Add(item);

			_suppressNotification = false;
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		public void SetRange(IEnumerable<T> list)
		{
			if (list == null)
				throw new ArgumentNullException("list");

			_suppressNotification = true;

			ClearItems();
			foreach (T item in list)
				Add(item);

			_suppressNotification = false;
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		public void ClearRange()
		{
			_suppressNotification = true;

			ClearItems();

			_suppressNotification = false;
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
	}

	public abstract class EntityViewModel<T> : ValidableViewModel, IConfirmNavigationRequest, IRegionMemberLifetime, INavigationAware
	where T : class, IHaveEntityProperty, IHaveNameProperty, IHaveIdProperty, IValidable
	{
		#region Constructor and Initialization
		protected EntityViewModel(IUnitOfWorkFactory uowFactory)
		{
			UnitOfWorkFactory = uowFactory;
			UnitOfWork = UnitOfWorkFactory.Create();

			InitializeCommands();
			InitializeEntities();
			InitializeFilter();
		}

		[Import]
		protected IInteractionService InteractionService { get; set; }

		[Import]
		public IRegionManager RegionManager { get; set; }

		protected void InitializeViewModel()
		{
			UnitOfWork = UnitOfWorkFactory.Create();
			Entities.CollectionChanged -= Entities_CollectionChanged;
			TryLoadEntities();

			//UpdateRecentEntities();
			UpdateCurrentSelection();

			Entities.CollectionChanged += Entities_CollectionChanged;
			UpdateProperties();
			UpdateDatabaseProperties();
		}

		protected void CleanupViewModel()
		{
			LastSelectedIndex = SelectedItem?.Id;
			_Entities.Clear();
			UnitOfWork.Dispose();
			UnitOfWork = null;
		}

		protected void ResetViewModel()
		{
			CleanupViewModel();
			InitializeViewModel();
		}
		#endregion

		#region Sorting
		public void ToggleSortDirection()
		{
			CurrentListSortDirection = CurrentListSortDirection == ListSortDirection.Ascending ?
				ListSortDirection.Descending :
				ListSortDirection.Ascending;
		}

		private IEnumerable<KeyValuePair<string, IEnumerable<SortDescription>>> GetDefaultSortDescriptions()
		{
			var tuples = typeof(T).GetProperties()
				.Where(pi => Attribute.IsDefined(pi, typeof(SortCriteriumAttribute)))
				.Select(pi => new Tuple<string, SortCriteriumAttribute>(
					pi.Name,
					((SortCriteriumAttribute)pi.GetCustomAttributes(typeof(SortCriteriumAttribute), true)[0])))
				.OrderBy(tuple => tuple.Item2.Order);
			return tuples
				.Select(tuple => new KeyValuePair<string, IEnumerable<SortDescription>>(
					tuple.Item2.Name,
					new[] { new SortDescription(tuple.Item1, tuple.Item2.Direction) }));
		}

		public ListSortDirection CurrentListSortDirection
		{
			get
			{
				return EntitiesView.SortDescriptions.Any() ?
					EntitiesView.SortDescriptions.First().Direction :
					ListSortDirection.Ascending;
			}
			set
			{
				var sortDescriptions = EntitiesView.SortDescriptions
					.Select(desc => new SortDescription(desc.PropertyName, value))
					.ToList();
				EntitiesView.SortDescriptions.Clear();
				EntitiesView.SortDescriptions.AddRange(sortDescriptions);
				OnPropertyChanged(nameof(CurrentListSortDirection));
			}
		}
		public IEnumerable<KeyValuePair<string, IEnumerable<SortDescription>>> AllSortDescriptions
		{
			get; private set;
		}

		public virtual IEnumerable<KeyValuePair<string, IEnumerable<SortDescription>>> CustomSortDescriptions
		{
			get
			{
				return Enumerable.Empty<KeyValuePair<string, IEnumerable<SortDescription>>>();
			}
		}

		public string CurrentUserSortDescription
		{
			get { return _currentUserSortDescription; }
			set
			{
				_currentUserSortDescription = value;
				EntitiesView.SortDescriptions.Clear();
				EntitiesView.SortDescriptions.AddRange(EntitiesSortDescriptions);
			}
		}
		private string _currentUserSortDescription;


		private void InitializeDefaultSortCriterium()
		{
			AllSortDescriptions = CustomSortDescriptions.Concat(GetDefaultSortDescriptions());
			CurrentUserSortDescription = AllSortDescriptions.Any() ? AllSortDescriptions.First().Key : string.Empty;
		}

		public virtual IEnumerable<SortDescription> EntitiesSortDescriptions
		{
			get
			{
				var result = AllSortDescriptions.FirstOrDefault(t => t.Key == _currentUserSortDescription);

				return result.Equals(default(KeyValuePair<string, IEnumerable<SortDescription>>)) ?
					Enumerable.Empty<SortDescription>() :
					result.Value;
			}
		}
		#endregion


		#region Filter
		public EntityFilter<T> UserFilter
		{
			get; protected set;
		}

		protected virtual EntityFilter<T> CreateFilter()
		{
			return new CustomEntityFilter<T>();
		}

		protected void InitializeFilter()
		{
			UserFilter = CreateFilter();
			(UserFilter as CustomEntityFilter<T>).InitializeFilter(EntitiesFilterDescriptions);
			UserFilter.ResetFilters();
			EntitiesView.Filter = UserFilter.Filter;

			UserFilter.FilterChanged += OnUserFilterChanged;
			OnPropertyChanged(nameof(UserFilter));
		}

		protected virtual void OnUserFilterChanged(object sender, EventArgs e)
		{
			EntitiesView.Refresh();
			RaiseCanExecuteChanged();
		}

		protected string FilterToString()
		{
			if (UserFilter == null) return string.Empty;
			var filterString = UserFilter.ToDisplayString();
			return string.IsNullOrEmpty(filterString) ? string.Empty : $"Filter: {filterString}";
		}

		protected virtual IEnumerable<IFilterDescription> EntitiesFilterDescriptions
		{
			get { return Enumerable.Empty<FilterDescription<T>>(); }
		}

		public bool CanClearFilter()
		{
			return UserFilter.CanClearFilters() || UserFilter.CanResetFilters();
		}

		public void ClearFilter()
		{
			UserFilter.ResetFilters();
			UserFilter.ClearFilters();
			OnPropertyChanged(nameof(UserFilter));
		}
		#endregion


		#region Entities

		public RangeObservableCollection<T> Entities
		{
			get { return _Entities; }
			set { SetProperty(ref _Entities, value); }
		}

		public RangeObservableCollection<T> _Entities = new RangeObservableCollection<T>();

		ICollectionView _entitiesView;
		public ICollectionView EntitiesView
		{
			get { return _entitiesView; }
		}

		private void InitializeEntities()
		{
			_entitiesView = CollectionViewSource.GetDefaultView(_Entities);
			(_entitiesView as ListCollectionView).IsLiveFiltering = true;
			(_entitiesView as ListCollectionView).IsLiveSorting = true;

			InitializeDefaultSortCriterium();

			EntitiesView.SortDescriptions.AddRange(EntitiesSortDescriptions);
			EntitiesView.CurrentChanged += EntitiesView_CurrentChanged;
			//EntitiesView.Refresh();
			//TryLoadEntities();
		}

		private void EntitiesView_CurrentChanged(object sender, EventArgs e)
		{
			OnPropertyChanged(nameof(StatusBarInfo));
			OnPropertyChanged(nameof(NumEntitiesInfo));
		}

		protected void Entities_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			UpdateProperties();
			OnEntitiesCollectionChanged();
		}

		protected void TryLoadEntities()
		{
			try
			{
				var entities = LoadEntities();
				_Entities.SetRange(entities);
			}
			catch (Exception e)
			{ // db error...
				_Entities.Clear();
			}
			OnPropertyChanged(nameof(EntitiesView));
		}

		public abstract IList<T> LoadEntities();

		protected virtual void OnEntitiesCollectionChanged() { }
		#endregion

		#region Recent Entities
		private IList<int> RecentEntitiesIndexes { get; set; } = new List<int>();

		public IList<T> RecentEntities
		{
			get
			{
				return (from id in RecentEntitiesIndexes
						join e in Entities on id equals e.Id
						select e).ToList();
			}
		}

		protected void UpdateRecentEntities()
		{
			OnPropertyChanged(nameof(RecentEntities));
			OnPropertyChanged(nameof(SelectedItem));

		}

		private void UpdateCurrentSelection()
		{
			if (LastSelectedIndex != null)
				SelectedItem = _Entities.FirstOrDefault(e => e.Id == LastSelectedIndex);
		}

		protected void AddToRecents(T entity)
		{
			if (entity == null) return;
			var entityIndex = entity.Id;
			var index = RecentEntitiesIndexes.IndexOf(entityIndex);
			if (index > -1)
			{
				RecentEntitiesIndexes.RemoveAt(index);
			}
			RecentEntitiesIndexes.Insert(0, entityIndex);

			if (RecentEntitiesIndexes.Count > 5) RecentEntitiesIndexes.RemoveAt(5);
			OnPropertyChanged(nameof(RecentEntities));
		}
		#endregion

		#region Selection
		public T SelectedItem
		{
			get { return _SelectedItem; }
			set
			{
				if (SetProperty(ref _SelectedItem, value))
				{
					if (value != null)
						value.ValidateInstance();
					OnPropertyChanged(nameof(DetailsTitle));
					RaiseCanExecuteChanged();
				}
				OnPropertyChanged(nameof(SelectedItem));

				//ContinueIfValidOrUserConfirmed(() =>
				//{
				//    if (SetProperty(ref _SelectedItem, value))
				//    {
				//        OnPropertyChanged(nameof(DetailsTitle));
				//        RaiseCanExecuteChanged();
				//    }
				//},
				//() => { OnPropertyChanged(nameof(SelectedItem)); }
				//);
			}
		}
		public T _SelectedItem;

		protected int? LastSelectedIndex { get; set; }
		#endregion


		#region Commands
		private void InitializeCommands()
		{
			AddCommand = new DelegateCommand(this.Add, this.CanAdd);
			ArchiveCommand = new DelegateCommand(this.Archive, this.CanArchive);
			RestoreCommand = new DelegateCommand(this.Restore, this.CanRestore);
			DeleteCommand = new DelegateCommand(this.Delete, this.CanDelete);

			ClearFilterCommand = new DelegateCommand(this.ClearFilter, this.CanClearFilter);
			ToggleSortDirectionCommand = new DelegateCommand(this.ToggleSortDirection);
			//EditCommand = new DelegateCommand(this.Edit, this.CanEdit);
			//CommitCommand = new DelegateCommand(this.Commit, this.CanCommit);
			//CancelCommand = new DelegateCommand(this.Cancel, this.CanCancel);
			BitmapRenderer = new BitmapRenderer(() => SelectedItem is IHaveDisplayNameProperty ? (SelectedItem as IHaveDisplayNameProperty).DisplayName : SelectedItem?.Name);
		}

		public DelegateCommand AddCommand { get; protected set; }
		public DelegateCommand ArchiveCommand { get; private set; }
		public DelegateCommand RestoreCommand { get; private set; }
		public DelegateCommand DeleteCommand { get; private set; }

		public DelegateCommand ClearFilterCommand { get; private set; }
		public DelegateCommand ToggleSortDirectionCommand { get; protected set; }

		public BitmapRenderer BitmapRenderer { get; private set; }
		//public DelegateCommand EditCommand { get; private set; }
		//public DelegateCommand CommitCommand { get; private set; }
		//public DelegateCommand CancelCommand { get; private set; }

		public virtual void RaiseCanExecuteChanged()
		{
			//CancelCommand.RaiseCanExecuteChanged();
			//CommitCommand.RaiseCanExecuteChanged();
			AddCommand.RaiseCanExecuteChanged();
			ArchiveCommand.RaiseCanExecuteChanged();
			RestoreCommand.RaiseCanExecuteChanged();
			DeleteCommand.RaiseCanExecuteChanged();
			ClearFilterCommand.RaiseCanExecuteChanged();
			//EditCommand.RaiseCanExecuteChanged();
		}

		public abstract string NewEntityName { get; }


		public virtual bool CanAdd() { return true; }
		public abstract void Add();

		public virtual bool CanArchive() { return SelectedItem != null && (SelectedItem is IHaveActiveProperty) && (SelectedItem as IHaveActiveProperty).Active; }
		public virtual void Archive()
		{
			InteractionService.ExecuteIfUserArchivingConfirmed(
				() =>
				{
					ArchiveInternal();
				});
		}
		protected void ArchiveInternal()
		{
			(SelectedItem as IHaveActiveProperty).Active = false;
			OnArchivingEntity(SelectedItem);
			CommitToDatabase();
			OnUserFilterChanged(null, EventArgs.Empty);
			RaiseCanExecuteChanged();
		}

		public virtual void OnArchivingEntity(T entity) { }


		public virtual bool CanRestore() { return SelectedItem != null && (SelectedItem is IHaveActiveProperty) && !(SelectedItem as IHaveActiveProperty).Active; }
		public void Restore()
		{
			(SelectedItem as IHaveActiveProperty).Active = true;
			OnRestoringEntity(SelectedItem);
			CommitToDatabase();
			OnUserFilterChanged(null, EventArgs.Empty);
			RaiseCanExecuteChanged();
		}
		public virtual void OnRestoringEntity(T entity) { }


		public virtual bool CanDelete()
		{
			return UnitOfWork != null && SelectedItem?.Entity != null;
		}

		public void Delete()
		{
			InteractionService.ExecuteIfUserDeletionConfirmed(
				() =>
				{
					UnitOfWork.DeleteEntity(SelectedItem.Entity);
					OnDeletingEntity(SelectedItem);
					CommitToDatabase();
					Entities.Remove(SelectedItem);
					OnUserFilterChanged(null, EventArgs.Empty);
					RaiseCanExecuteChanged();
					SelectedItem = null;
				}
			);
		}

		public virtual void OnDeletingEntity(T entity) { }
		#endregion


		#region Statistics and Informations
		public int NumberActiveEntities { get { return Entities.Count(p => p is IHaveActiveProperty && (p as IHaveActiveProperty).Active); } }
		public int NumberInactiveEntities { get { return Entities.Count(p => p is IHaveActiveProperty &&  !(p as IHaveActiveProperty).Active); } }
		public int NumberFilteredEntities { get { return EntitiesView.Cast<object>().Count(); } }
		public int NumberTotalEntities { get { return Entities.Count(); } }

		public int DatabaseVersion
		{
			get { return UnitOfWork != null ? UnitOfWork.DatabaseVersion : -1; }
		}

		public string NumEntitiesInfo
		{
			get { return $"{NumberFilteredEntities} / {NumberTotalEntities} ({NumberInactiveEntities})"; }
		}

		public virtual string StatusBarInfo
		{
			get { return UserFilter.ToString(); }
		}

		public abstract string DetailsHeader(T entity);

		public string DetailsTitle
		{
			get { return DetailsHeader(SelectedItem); }
		}
		#endregion


		#region Property Changed Handling
		protected void OnNotifyPropertyChanged(Object sender, PropertyChangedEventArgs e)
		{
			if (IsEntityProperty(sender, e.PropertyName))
			{
				var vm = sender as INotifyDataErrorInfo;
				if (vm != null)
				{
					if (vm.GetErrors(e.PropertyName).Cast<object>().Count() != 0)
						return;
				}
				CommitToDatabase(ValidationErrorHandler(vm, e.PropertyName));
				AddToRecents(SelectedItem);
				UpdateProperties();
			}
			OnNotifyPropertyChanged(e);
		}

		protected delegate void GenerateValidationErrorHandler(Exception e);

		protected GenerateValidationErrorHandler ValidationErrorHandler(INotifyDataErrorInfo data, string propertyName)
		{
			return e => {
				if (data is ValidableViewModel)
					(data as ValidableViewModel).SetValidationError(propertyName, e);
				OnPropertyChanged(propertyName);
			};
		}

		protected void CommitToDatabase()
		{
			GenerateValidationErrorHandler errorHandler =
				e => {
					InteractionService.RaiseNotificationAsync($"Datenbank-Befehl konnte nicht ausgeführt werden:\r\n{e.Message}", "Fehler");
					TryRefreshAll();
				};
			CommitToDatabase(errorHandler);
		}

		private void TryRefreshAll()
		{
			try
			{
				UnitOfWork.RefreshAll();
			}
			catch { }
		}

		protected void CommitToDatabase(GenerateValidationErrorHandler errorHandler)
		{
			try
			{
				UnitOfWork.Complete();
			}
			catch (Exception e)
			{
				var exception = e.InnerException ?? e;
				errorHandler?.Invoke(exception);
			}
		}

		private bool IsEntityProperty(object obj, string propertyName)
		{
			var prop = obj != null ? obj.GetType().GetProperty(propertyName) : null;
			return prop != null && prop.GetCustomAttributes(typeof(ComputedPropertyAttribute), false).Length == 0;
		}

		protected virtual void OnNotifyPropertyChanged(PropertyChangedEventArgs e) { }

		private void UpdateDatabaseProperties()
		{
			OnPropertyChanged(nameof(DatabaseVersion));
		}

		private void UpdateProperties()
		{
			OnPropertyChanged(nameof(DetailsTitle));
			OnPropertyChanged(nameof(Entities));
			OnPropertyChanged(nameof(EntitiesView));
			//OnPropertyChanged(nameof(SelectedItem)); // needed?
			OnPropertyChanged(nameof(RecentEntities));

			OnPropertyChanged(nameof(NumberActiveEntities));
			OnPropertyChanged(nameof(NumberInactiveEntities));

			OnPropertyChanged(nameof(StatusBarInfo));
			OnPropertyChanged(nameof(NumEntitiesInfo));
		}
		#endregion


		#region IRegionMemberLifetime
		public bool KeepAlive
		{
			get
			{
				return true;
			}
		}
		#endregion


		#region INavigationAware
		public virtual void OnNavigatedTo(NavigationContext navigationContext)
		{
			InitializeViewModel();
			SelectEntityIfIdProvided(navigationContext);
			CreateNewIfTagProvided(navigationContext);
		}

		private void CreateNewIfTagProvided(NavigationContext navigationContext)
		{
			var add = navigationContext.Parameters["NEW"];
			if (add != null)
				AddCommand.Execute();
		}

		private void SelectEntityIfIdProvided(NavigationContext navigationContext)
		{
			var id = navigationContext.Parameters["ID"];
			if (id != null)
				SelectedItem = _Entities.FirstOrDefault(j => j.Id == (int)id);
		}

		public bool IsNavigationTarget(NavigationContext navigationContext)
		{
			return true;
		}

		public virtual void OnNavigatedFrom(NavigationContext navigationContext)
		{
			CleanupViewModel();
		}
		#endregion


		#region Validation
		//public override void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
		//{
		//    ContinueIfValidOrUserConfirmed(() => continuationCallback(true), () => { });
		//}

		//public void ContinueIfValidOrUserConfirmed(Action continuationCallback, Action fallbackCallback)
		//{
		//    if (SelectedItem != null && !SelectedItem.ValidateInstance())
		//    {
		//        string errors = SelectedItem.ValidationErrorsAsString();

		//        InteractionService.ExecuteIfUserConfirmed(
		//            "Überprüfungsfehler",
		//            $"Ein oder mehrere Felder können nicht validiert werden, weil die folgende Überprüfungsanforderungen nicht erfüllt werden:\n\n{errors}\n\nMöchten Sie trotzdem die Eingabemaske verlassen?",
		//            continuationCallback,
		//            fallbackCallback);
		//    }
		//    else
		//    {
		//        continuationCallback();
		//    }
		//}
		#endregion

		#region Workaround https://github.com/MahApps/MahApps.Metro/issues/2622 Fix this with Mahapps 1.6
		public CultureInfo CustomCulture
		{
			get
			{
				if (_ci == null)
				{
					_ci = new CultureInfo("de-DE");
					_ci.DateTimeFormat.LongTimePattern = _ci.DateTimeFormat.ShortTimePattern;
				}
				return _ci;
			}
		}
		private CultureInfo _ci;
		#endregion


		#region Fields
		protected IUnitOfWorkFactory UnitOfWorkFactory;
		public IUnitOfWork UnitOfWork;
		#endregion
	}
}
