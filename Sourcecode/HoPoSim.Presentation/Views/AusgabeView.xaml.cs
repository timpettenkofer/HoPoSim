using HoPoSim.Presentation.ViewModels;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace HoPoSim.Presentation.Views
{
	[Export(typeof(AusgabeView))]
	public partial class AusgabeView : UserControl
	{
		[DllImport("User32.dll")]
		static extern bool MoveWindow(IntPtr handle, int x, int y, int width, int height, bool redraw);

		internal delegate int WindowEnumProc(IntPtr hwnd, IntPtr lparam);
		[DllImport("user32.dll")]
		internal static extern bool EnumChildWindows(IntPtr hwnd, WindowEnumProc func, IntPtr lParam);

		[DllImport("user32.dll")]
		static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

		private Process process;
		private IntPtr unityHWND = IntPtr.Zero;

		private const int WM_ACTIVATE = 0x0006;
		private readonly IntPtr WA_ACTIVE = new IntPtr(1);
		private readonly IntPtr WA_INACTIVE = new IntPtr(0);

		private System.Windows.Forms.Panel hostPanel = null;

		public AusgabeView()
		{
			InitializeComponent();
			//hostBorder.SizeChanged += HostBorder_SizeChanged;
			//hostPanel = (windowsFormsHost.Child as System.Windows.Forms.Panel);
			//hostPanel.Resize += hostPanel_Resize;
			//this.Unloaded += AusgabeView_Unloaded;
			//this.Loaded += AusgabeView_Loaded;
		}


		//private void ActivateUnityWindow()
		//{
		//	SendMessage(unityHWND, WM_ACTIVATE, WA_ACTIVE, IntPtr.Zero);
		//}

		//private void DeactivateUnityWindow()
		//{
		//	SendMessage(unityHWND, WM_ACTIVATE, WA_INACTIVE, IntPtr.Zero);
		//}


		public Process StartProcess()
		{
			try
			{
				var vm = DataContext as AusgabeViewModel;
				process = vm.StartUnityProcess(hostPanel.Handle);

				//EnumChildWindows(hostPanel.Handle, WindowEnum, IntPtr.Zero);
				return process;

			}
			catch (Exception e)
			{
				System.Windows.MessageBox.Show(e.Message + ".\nError while starting 3D Viewer.");
				return null;
			}
		}


		//private int WindowEnum(IntPtr hwnd, IntPtr lparam)
		//{
		//	unityHWND = hwnd;
		//	ActivateUnityWindow();
		//	return 0;
		//}

		//public void KillProcess(Process process)
		//{
		//	try
		//	{
		//		if (process == null || process.HasExited)
		//			return;

		//		process.CloseMainWindow();

		//		Thread.Sleep(1000);
		//		while (!process.HasExited)
		//			process.Kill();
		//	}
		//	catch (Exception e)
		//	{
		//		System.Windows.MessageBox.Show(e.Message + ".\nError while killing 3D Viewer.");
		//	}
		//}

		//private void AusgabeView_Loaded(object sender, RoutedEventArgs e)
		//{
		//	ActivateUnityWindow();
		//	if (window == null)
		//	{
		//		window = Window.GetWindow(this);
		//		window.Closing += Window_Closing;
		//	}

		//	//Dispatcher.BeginInvoke(DispatcherPriority.Normal,new Action(delegate (){ hostBorder.Focus(); }));
		//}

		//private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		//{
		//	KillProcess(process);
		//}

		//private void AusgabeView_Unloaded(object sender, RoutedEventArgs e)
		//{
		//	DeactivateUnityWindow();
		//}


		//private void HostBorder_SizeChanged(object sender, SizeChangedEventArgs e)
		//{
		//	var border = sender as Border;
		//	windowsFormsHost.Width = border.ActualWidth;
		//	windowsFormsHost.Height = border.ActualHeight;
		//}

		//private void hostPanel_Resize(object sender, EventArgs e)
		//{
		//	MoveWindow(unityHWND, 0, 0, (int)hostPanel.Width, (int)hostPanel.Height, true);
		//}


		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var vm = DataContext as AusgabeViewModel;
			vm.SendMessage("Welcome client");
		}

		//public async void DelayInvoke(int millisecondsDelay, Action action)
		//{
		//	await Task.Delay(millisecondsDelay);
		//	await Dispatcher.BeginInvoke(action);
		//}

		//private Window window = null;
		private void StartViewer_Click(object sender, RoutedEventArgs e)
		{
			if (process != null && process.HasExited)
				process = null;
			if (process == null)
			{
				process = StartProcess();
			//	DelayInvoke(1000, new Action(() => { hostPanel_Resize(hostPanel, null); windowsFormsHost.Visibility = Visibility.Visible; }));
			}
		}
	}
}

