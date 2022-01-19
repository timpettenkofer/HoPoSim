using System;
using System.Windows;
using System.Windows.Controls;

namespace HoPoSim.Presentation.Controls
{
	[TemplatePart(Name = "PART_DetailsScrollViewer", Type = typeof(ScrollViewer))]
	public class EntityContentControl : Control
	{
		static EntityContentControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(EntityContentControl), new FrameworkPropertyMetadata(typeof(EntityContentControl)));
		}

		#region DetailsScrollViewer
		ScrollViewer DetailsScrollViewer;

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (this.Template != null)
			{
				DetailsScrollViewer = this.Template.FindName("PART_DetailsScrollViewer", this) as ScrollViewer;
			}
		}

		public void ScrollDetailsToHome()
		{
			if (DetailsScrollViewer != null)
				DetailsScrollViewer.ScrollToHome();
		}
		#endregion

		public string EntityName
		{
			get { return (string)GetValue(EntityNameProperty); }
			set { SetValue(EntityNameProperty, value); }
		}

		public static readonly DependencyProperty EntityNameProperty =
		  DependencyProperty.Register("EntityName", typeof(string), typeof(EntityContentControl),
		  new UIPropertyMetadata(null));

		public bool HideActiveFilter
		{
			get { return (bool)GetValue(HideActiveFilterProperty); }
			set { SetValue(HideActiveFilterProperty, value); }
		}

		public static readonly DependencyProperty HideActiveFilterProperty =
		  DependencyProperty.Register("HideActiveFilter", typeof(bool), typeof(EntityContentControl),
		  new UIPropertyMetadata(false));

		public bool HideDecoration
		{
			get { return (bool)GetValue(HideDecorationProperty); }
			set { SetValue(HideDecorationProperty, value); }
		}

		public static readonly DependencyProperty HideDecorationProperty =
		  DependencyProperty.Register("HideDecoration", typeof(bool), typeof(EntityContentControl),
		  new UIPropertyMetadata(false));


		public Object BrowserCommandPanel
		{
			get { return (Object)GetValue(BrowserCommandPanelProperty); }
			set { SetValue(BrowserCommandPanelProperty, value); }
		}

		public static readonly DependencyProperty BrowserCommandPanelProperty =
		  DependencyProperty.Register("BrowserCommandPanel", typeof(Object), typeof(EntityContentControl),
		  new UIPropertyMetadata(null));


		public Object CustomBrowserFilterPanel
		{
			get { return (Object)GetValue(CustomBrowserFilterPanelProperty); }
			set { SetValue(CustomBrowserFilterPanelProperty, value); }
		}

		public static readonly DependencyProperty CustomBrowserFilterPanelProperty =
		  DependencyProperty.Register("CustomBrowserFilterPanel", typeof(Object), typeof(EntityContentControl),
		  new UIPropertyMetadata(null));


		public Object BrowserControl
		{
			get { return (Object)GetValue(BrowserControlProperty); }
			set { SetValue(BrowserControlProperty, value); }
		}

		public static readonly DependencyProperty BrowserControlProperty =
		  DependencyProperty.Register("BrowserControl", typeof(Object), typeof(EntityContentControl),
		  new UIPropertyMetadata(null));

		public Object DetailsPanel
		{
			get { return (Object)GetValue(DetailsPanelProperty); }
			set { SetValue(DetailsPanelProperty, value); }
		}

		public static readonly DependencyProperty DetailsPanelProperty =
		  DependencyProperty.Register("DetailsPanel", typeof(Object), typeof(EntityContentControl),
		  new UIPropertyMetadata(null));

		public Object SummaryPanel
		{
			get { return (Object)GetValue(SummaryPanelProperty); }
			set { SetValue(SummaryPanelProperty, value); }
		}

		public static readonly DependencyProperty SummaryPanelProperty =
		  DependencyProperty.Register("SummaryPanel", typeof(Object), typeof(EntityContentControl),
		  new UIPropertyMetadata(null));

		public Object OutputPanel
		{
			get { return (Object)GetValue(OutputPanelProperty); }
			set { SetValue(OutputPanelProperty, value); }
		}

		public static readonly DependencyProperty OutputPanelProperty =
		  DependencyProperty.Register("OutputPanel", typeof(Object), typeof(EntityContentControl),
		  new UIPropertyMetadata(null));

		public string OutputPanelHeader
		{
			get { return (string)GetValue(OutputPanelHeaderProperty); }
			set { SetValue(OutputPanelHeaderProperty, value); }
		}

		public static readonly DependencyProperty OutputPanelHeaderProperty =
		  DependencyProperty.Register("OutputPanelHeader", typeof(string), typeof(EntityContentControl).BaseType,
		  new UIPropertyMetadata("Output"));

	}
}
