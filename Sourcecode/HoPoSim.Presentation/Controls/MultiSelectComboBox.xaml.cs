using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace HoPoSim.Presentation.Controls
{
	public partial class MultiSelectComboBox : UserControl
	{
		private ObservableCollection<Node> _nodeList;
		public MultiSelectComboBox()
		{
			InitializeComponent();
			DefaultText = SelectAllString;
			_nodeList = new ObservableCollection<Node>();
		}

		#region Dependency Properties

		public static readonly DependencyProperty ItemsSourceProperty =
			 DependencyProperty.Register("ItemsSource", typeof(Dictionary<object, string>), typeof(MultiSelectComboBox), new FrameworkPropertyMetadata(null,
		new PropertyChangedCallback(MultiSelectComboBox.OnItemsSourceChanged)));

		public static readonly DependencyProperty SelectedItemsProperty =
		 DependencyProperty.Register("SelectedItems", typeof(Dictionary<object, string>), typeof(MultiSelectComboBox),
			 new FrameworkPropertyMetadata(new Dictionary<object, string>(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(MultiSelectComboBox.OnSelectedItemsChanged)));

		public static readonly DependencyProperty TextProperty =
		   DependencyProperty.Register("Text", typeof(string), typeof(MultiSelectComboBox), new UIPropertyMetadata(string.Empty));

		public static readonly DependencyProperty DefaultTextProperty =
			DependencyProperty.Register("DefaultText", typeof(string), typeof(MultiSelectComboBox), new UIPropertyMetadata(string.Empty));


		public Dictionary<object, string> ItemsSource
		{
			get { return (Dictionary<object, string>)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		public Dictionary<object, string> SelectedItems
		{
			get { return (Dictionary<object, string>)GetValue(SelectedItemsProperty); }
			set { SetValue(SelectedItemsProperty, value); }
		}

		private Dictionary<object, string> InternalSelectedItems
		{
			get; set;
		}

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		public string DefaultText
		{
			get { return (string)GetValue(DefaultTextProperty); }
			set { SetValue(DefaultTextProperty, value); }
		}
		#endregion

		#region Events
		private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			MultiSelectComboBox control = (MultiSelectComboBox)d;
			control.DisplayInControl();
			control.SynchronizeSelection();
		}

		private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			MultiSelectComboBox control = (MultiSelectComboBox)d;
			control.InternalSelectedItems = control.SelectedItems;
			control.SynchronizeSelection();
			control.SetText();
		}

		private void CheckBox_Click(object sender, RoutedEventArgs e)
		{
			CheckBox clickedBox = (CheckBox)sender;

			if (SelectAllString.Equals(clickedBox.Content))
			{
				if (clickedBox.IsChecked.Value)
				{
					foreach (Node node in _nodeList)
						node.IsSelected = true;
				}
				else
				{
					foreach (Node node in _nodeList)
						node.IsSelected = false;
				}
			}
			else
			{
				int _selectedCount = 0;
				foreach (Node s in _nodeList)
				{
					if (s.IsSelected && s.Title != SelectAllString)
						_selectedCount++;
				}
				_nodeList.FirstOrDefault(i => i.Title == SelectAllString).IsSelected = (_selectedCount == _nodeList.Count - 1);
			}
			SetSelectedItems();
			SelectedItems = InternalSelectedItems;
			SetText();

		}

		public static readonly RoutedEvent DropDownOpenedEvent =
			EventManager.RegisterRoutedEvent("DropDownOpened", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MultiSelectComboBox));

		public event RoutedEventHandler DropDownOpened
		{
			add { AddHandler(DropDownOpenedEvent, value); }
			remove { RemoveHandler(DropDownOpenedEvent, value); }
		}

		void RaiseDropDownOpenedEvent()
		{
			RoutedEventArgs newEventArgs = new RoutedEventArgs(MultiSelectComboBox.DropDownOpenedEvent);
			RaiseEvent(newEventArgs);
		}

		private void MultiSelectCombo_DropDownOpened(object sender, System.EventArgs e)
		{
			RaiseDropDownOpenedEvent();
		}
		#endregion


		#region Methods
		private void SelectNodes()
		{
			if (InternalSelectedItems == null) return;
			foreach (KeyValuePair<object, string> keyValue in InternalSelectedItems)
			{
				Node node = _nodeList.FirstOrDefault(i => i.Title == keyValue.Value);
				if (node != null)
					node.IsSelected = true;
			}
			SelectAllNodeIfSingleNotSelected();
		}

		private void SelectAllNodeIfSingleNotSelected()
		{
			var notSelected = _nodeList.Where(n => !n.IsSelected);
			if (notSelected.Count() == 1 && notSelected.First().Title.Equals(SelectAllString))
				notSelected.First().IsSelected = true;
		}

		private string SelectAllString = "Alle";

		private void SelectAllNodes()
		{
			if (InternalSelectedItems == null)
			{
				foreach (Node node in _nodeList)
					node.IsSelected = true;
				SetSelectedItems();
			}
		}

		public void SynchronizeSelection()
		{
			if (InternalSelectedItems == null)
				SelectAllNodes();
			else
				SelectNodes();
		}

		private void SetSelectedItems()
		{
			if (InternalSelectedItems == null)
				InternalSelectedItems = new Dictionary<object, string>();
			InternalSelectedItems.Clear();
			var newSelectedItems = new Dictionary<object, string>();
			foreach (Node node in _nodeList)
			{
				if (node.IsSelected && node.Title != SelectAllString)
				{
					if (this.ItemsSource.Count > 0)
						newSelectedItems.Add(node.Id, node.Title);
				}
			}
			InternalSelectedItems = newSelectedItems;
		}

		private void DisplayInControl()
		{
			_nodeList.Clear();
			if (this.ItemsSource.Count > 0)
				_nodeList.Add(new Node(-1, SelectAllString));
			foreach (KeyValuePair<object, string> keyValue in this.ItemsSource)
			{
				Node node = new Node(keyValue.Key, keyValue.Value);
				_nodeList.Add(node);
			}
			MultiSelectCombo.ItemsSource = _nodeList;
		}

		private void SetText()
		{
			this.Text = this.DefaultText;

			if (this.InternalSelectedItems != null)
			{
				StringBuilder displayText = new StringBuilder();
				foreach (Node s in _nodeList)
				{
					if (s.IsSelected == true && s.Title == SelectAllString)
					{
						displayText = new StringBuilder();
						displayText.Append(SelectAllString);
						break;
					}
					else if (s.IsSelected == true && s.Title != SelectAllString)
					{
						displayText.Append(s.Title);
						displayText.Append(',');
					}
				}
				this.Text = displayText.ToString().TrimEnd(new char[] { ',' });
			}
		}
		#endregion
	}

	public class Node : INotifyPropertyChanged
	{
		private string _title;
		private bool _isSelected;
		#region ctor
		public Node(object id, string title)
		{
			Id = id;
			Title = title;
		}
		#endregion

		#region Properties
		public string Title
		{
			get
			{
				return _title;
			}
			set
			{
				_title = value;
				NotifyPropertyChanged("Title");
			}
		}

		public object Id { get; set; }

		public bool IsSelected
		{
			get
			{
				return _isSelected;
			}
			set
			{
				_isSelected = value;
				NotifyPropertyChanged("IsSelected");
			}
		}
		#endregion

		public event PropertyChangedEventHandler PropertyChanged;
		protected void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

	}
}
