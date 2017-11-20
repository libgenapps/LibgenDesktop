using System.Windows;
using LibgenDesktop.Infrastructure;

namespace LibgenDesktop
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            IWindowContext mainWindowContext = WindowManager.CreateWindow(RegisteredWindows.WindowKey.MAIN_WINDOW);
            mainWindowContext.Closed += (sender, args) => Shutdown();
            mainWindowContext.Show();
        }
    }
}
