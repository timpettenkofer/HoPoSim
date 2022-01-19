using MahApps.Metro.IconPacks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HoPoSim.Presentation.Controls
{
	[TemplatePart(Name = "PART_EntityCommandButton", Type = typeof(Button))]
	public class EntityCommandControl : Control
	{
		static EntityCommandControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(EntityCommandControl), new FrameworkPropertyMetadata(typeof(EntityCommandControl)));
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (this.Template != null)
			{
				var entityCommandButton = this.Template.FindName("PART_EntityCommandButton", this) as Button;
				if (entityCommandButton != EntityCommandButton)
				{
					if (EntityCommandButton != null)
					{
						EntityCommandButton.Click -= EntityCommandButton_Click;
					}
					EntityCommandButton = entityCommandButton;
					if (EntityCommandButton != null)
					{
						EntityCommandButton.Click += EntityCommandButton_Click;
					}
				}
			}
		}

		private void EntityCommandButton_Click(object sender, RoutedEventArgs e)
		{
			OnCommandTriggered();
		}

		private static Button EntityCommandButton;

		public static readonly RoutedEvent CommandTriggeredEvent =
			EventManager.RegisterRoutedEvent("CommandTriggered", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(EntityCommandControl));

		public event RoutedEventHandler CommandTriggered
		{
			add { AddHandler(CommandTriggeredEvent, value); }
			remove { RemoveHandler(CommandTriggeredEvent, value); }
		}

		private void OnCommandTriggered()
		{
			RoutedEventArgs args = new RoutedEventArgs(CommandTriggeredEvent);
			RaiseEvent(args);
		}

		public ICommand Command
		{
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}

		public static readonly DependencyProperty CommandProperty =
		  DependencyProperty.Register("Command", typeof(ICommand), typeof(EntityCommandControl),
		  new UIPropertyMetadata(null));

		public object CommandParameter
		{
			get { return (object)GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}

		public static readonly DependencyProperty CommandParameterProperty =
		  DependencyProperty.Register("CommandParameter", typeof(object), typeof(EntityCommandControl),
		  new UIPropertyMetadata(null));

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		public static readonly DependencyProperty TextProperty =
		  DependencyProperty.Register("Text", typeof(string), typeof(EntityCommandControl),
		  new UIPropertyMetadata(null));

		public string Tooltip
		{
			get { return (string)GetValue(TooltipProperty); }
			set { SetValue(TooltipProperty, value); }
		}

		public static readonly DependencyProperty TooltipProperty =
		  DependencyProperty.Register("Tooltip", typeof(string), typeof(EntityCommandControl),
		  new UIPropertyMetadata(null));

		public PackIconMaterialKind IconMaterialKind
		{
			get { return (PackIconMaterialKind)GetValue(IconMaterialKindProperty); }
			set { SetValue(IconMaterialKindProperty, value); }
		}

		public static readonly DependencyProperty IconMaterialKindProperty =
		  DependencyProperty.Register("IconMaterialKind", typeof(PackIconMaterialKind), typeof(EntityCommandControl),
		  new UIPropertyMetadata(null));

	}
}
