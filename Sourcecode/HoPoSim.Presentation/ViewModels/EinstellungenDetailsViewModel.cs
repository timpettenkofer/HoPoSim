using HoPoSim.Data.Interfaces;
using HoPoSim.Data.Repositories;
using HoPoSim.Framework.Interfaces;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace HoPoSim.Presentation.ViewModels
{
	public interface IEinstellungenDetailsViewModel
	{
		void Load(IUnitOfWork uow);
		DelegateCommand AddCommand { get; }
		DelegateCommand DeleteCommand { get; }
		object SelectedItem { get; }
	}

	public class EinstellungenDetailsViewModel<VM, T> : IEinstellungenDetailsViewModel where VM : class, ITypeDetailsViewModel<T> where T : class //, IHaveStringValueProperty
	{
		public DelegateCommand AddCommand { get; private set; }
		public DelegateCommand DeleteCommand { get; private set; }

		public List<VM> _items;
		public ObservableCollection<VM> ItemsView
		{
			get; set;
		}

		IInteractionService _interaction;

		public EinstellungenDetailsViewModel(
			IInteractionService interaction,
			PropertyChangedEventHandler onPropertyChanged,
			Func<IUnitOfWork, IBaseRepository<T>> getRepository,
			Func<bool> commit,
			Func<IUnitOfWork, IEnumerable<T>> getItems = null)
		{
			_onPropertyChanged = onPropertyChanged;
			_interaction = interaction;
			_commit = commit;

			_items = new List<VM>();
			ItemsView = new ObservableCollection<VM>(_items);

			AddCommand = new DelegateCommand(this.Add, this.CanAdd);
			DeleteCommand = new DelegateCommand(this.Delete, this.CanDelete);

			GetRepository = getRepository;
			GetItems = getItems;
		}
		Func<bool> _commit;
		IUnitOfWork _uow;
		PropertyChangedEventHandler _onPropertyChanged;

		private Func<IUnitOfWork, IBaseRepository<T>> GetRepository { get; }

		private Func<IUnitOfWork, IEnumerable<T>> GetItems { get; }

		public void Load(IUnitOfWork uow)
		{
			_uow = uow;
			ItemsView.Clear();
			foreach (var p in GetItems != null ? GetItems(_uow) : GetRepository(_uow).GetAll())
			{
				var vm = (VM)Activator.CreateInstance(typeof(VM), new[] { p });
				vm.PropertyChanged += _onPropertyChanged;
				ItemsView.Add(vm);
			}
			RaiseCanExecuteChanged();
		}

		public object SelectedItem
		{
			get { return _selectedItem; }
			set
			{
				_selectedItem = value;
				RaiseCanExecuteChanged();
			}
		}
		private object _selectedItem;


		public void RaiseCanExecuteChanged()
		{
			AddCommand.RaiseCanExecuteChanged();
			DeleteCommand.RaiseCanExecuteChanged();
		}


		public bool CanDelete()
		{
			return _uow != null && SelectedItem is VM;
		}

		public void Delete()
		{
			_interaction.ExecuteIfUserArchivingConfirmed(
				() =>
				{
					var p = SelectedItem as VM;
					if (p != null)
					{
						p.PropertyChanged -= _onPropertyChanged;

						GetRepository(_uow).Remove(p.Source);
						_commit();
					}
				}
			);
		}

		public bool CanAdd()
		{
			return _uow != null && true;
		}

		public void Add()
		{
			var item = (T)Activator.CreateInstance(typeof(T), new[] { "neuer Typ" });
			var vm = (VM)Activator.CreateInstance(typeof(VM), new[] { item });
			vm.PropertyChanged += _onPropertyChanged;
			GetRepository(_uow).Add(item);
			_commit();
		}
	}
}
