using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LibgenDesktop.Infrastructure;

namespace LibgenDesktop.ViewModels
{
    internal abstract class ViewModel : INotifyPropertyChanged
    {
        protected IWindowContext CurrentWindowContext
        {
            get
            {
                return WindowManager.GetWindowContext(this);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void ShowErrorWindow(Exception exception)
        {
            IWindowContext currentWindowContext = WindowManager.GetWindowContext(this);
            ErrorWindowViewModel errorWindowViewModel = new ErrorWindowViewModel(exception.ToString());
            IWindowContext errorWindowContext = WindowManager.CreateWindow(RegisteredWindows.WindowKey.ERROR_WINDOW, errorWindowViewModel, currentWindowContext);
            errorWindowContext.ShowDialog();
        }
    }
}
