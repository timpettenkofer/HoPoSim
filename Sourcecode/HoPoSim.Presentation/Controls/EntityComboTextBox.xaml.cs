using HoPoSim.Data.Domain;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace HoPoSim.Presentation.Controls
{
	/// <summary>
	/// Interaction logic for EntityComboTextBox.xaml
	/// </summary>
	public partial class EntityComboTextBox : UserControl
	{
		public EntityComboTextBox()
		{
			InitializeComponent();
		}

		public Object EntityEditor
		{
			get { return (Object)GetValue(EntityEditorProperty); }
			set { SetValue(EntityEditorProperty, value); }
		}

		public static readonly DependencyProperty EntityEditorProperty =
		  DependencyProperty.Register("EntityEditor", typeof(Object), typeof(EntityComboTextBox),
		  new UIPropertyMetadata(null));

		public IEnumerable ComboBoxItemsSource
		{
			get { return (IEnumerable)GetValue(ComboBoxItemsSourceProperty); }
			set { SetValue(ComboBoxItemsSourceProperty, value); }
		}

		public static readonly DependencyProperty ComboBoxItemsSourceProperty =
		  DependencyProperty.Register("ComboBoxItemsSource", typeof(IEnumerable), typeof(EntityComboTextBox),
		  new UIPropertyMetadata(null));

		public Object ComboBoxSelectedValue
		{
			get { return (Object)GetValue(ComboBoxSelectedValueProperty); }
			set { SetValue(ComboBoxSelectedValueProperty, value); }
		}

		public static readonly DependencyProperty ComboBoxSelectedValueProperty =
		  DependencyProperty.Register("ComboBoxSelectedValue", typeof(Object), typeof(EntityComboTextBox),
		  new UIPropertyMetadata(null));


		public DataTemplate ComboBoxItemTemplate
		{
			get { return (DataTemplate)GetValue(ComboBoxItemTemplateProperty); }
			set { SetValue(ComboBoxItemTemplateProperty, value); }
		}

		public static readonly DependencyProperty ComboBoxItemTemplateProperty =
		  DependencyProperty.Register("ComboBoxItemTemplate", typeof(DataTemplate), typeof(EntityComboTextBox),
		  new UIPropertyMetadata(null));

		public IEntity TargetEntity
		{
			get { return (IEntity)GetValue(TargetEntityProperty); }
			set { SetValue(TargetEntityProperty, value); }
		}

		public static readonly DependencyProperty TargetEntityProperty =
		  DependencyProperty.Register("TargetEntity", typeof(IEntity), typeof(EntityComboTextBox),
		  new UIPropertyMetadata(null));


		public string TextBoxDisplayName
		{
			get { return (string)GetValue(TextBoxDisplayNameProperty); }
			set { SetValue(TextBoxDisplayNameProperty, value); }
		}

		public static readonly DependencyProperty TextBoxDisplayNameProperty =
		  DependencyProperty.Register("TextBoxDisplayName", typeof(string), typeof(EntityComboTextBox),
		  new UIPropertyMetadata(null));

		public string DisplayMemberPath
		{
			get { return (string)GetValue(DisplayMemberPathProperty); }
			set { SetValue(DisplayMemberPathProperty, value); }
		}

		public static readonly DependencyProperty DisplayMemberPathProperty =
		  DependencyProperty.Register("DisplayMemberPath", typeof(string), typeof(EntityComboTextBox),
		  new UIPropertyMetadata(null));
	}
}
