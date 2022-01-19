using MahAppsMetroThemesSample;
using System;
using System.Windows;

namespace HoPoSim
{
	public partial class App : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("de-DE");

            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionEventHandler;

            ThemeManagerHelper.RestoreTheme();
            ThemeManagerHelper.RestoreAccent();

            base.OnStartup(e);

            Bootstrapper bs = new Bootstrapper();
            bs.Run();
        }

        private void UnhandledExceptionEventHandler(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.IsTerminating)
            {
                object o = e.ExceptionObject;
                MessageBox.Show(o.ToString());
            }
        }
    }
}
