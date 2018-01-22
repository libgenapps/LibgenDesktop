using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LibgenDesktop.Common;
using LibgenDesktop.Infrastructure;

namespace LibgenDesktop.ViewModels
{
    internal abstract class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void ShowErrorWindow(Exception exception, IWindowContext parentWindowContext)
        {
            Logger.Exception(exception);
            ErrorWindowViewModel errorWindowViewModel = new ErrorWindowViewModel(exception.ToString());
            IWindowContext errorWindowContext = WindowManager.CreateWindow(RegisteredWindows.WindowKey.ERROR_WINDOW, errorWindowViewModel, parentWindowContext);
            errorWindowContext.ShowDialog();
        }
    }
}
