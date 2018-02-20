using System;
using LibgenDesktop.Common;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.ViewModels.Windows;

namespace LibgenDesktop.ViewModels
{
    internal abstract class ContainerViewModel : ViewModel
    {
        protected ContainerViewModel(MainModel mainModel)
        {
            MainModel = mainModel;
        }

        protected MainModel MainModel { get; }

        protected void ShowErrorWindow(Exception exception, IWindowContext parentWindowContext)
        {
            try
            {
                Logger.Exception(exception);
            }
            catch
            {
            }
            ErrorWindowViewModel errorWindowViewModel = new ErrorWindowViewModel(exception.ToString(), MainModel.Localization.CurrentLanguage);
            IWindowContext errorWindowContext = WindowManager.CreateWindow(RegisteredWindows.WindowKey.ERROR_WINDOW, errorWindowViewModel, parentWindowContext);
            errorWindowContext.ShowDialog();
        }

        protected virtual void ShowMessage(string title, string text, IWindowContext parentWindowContext)
        {
            string ok = MainModel.Localization.CurrentLanguage.MessageBox.Ok;
            WindowManager.ShowMessage(title, text, ok, parentWindowContext);
        }

        protected virtual bool ShowPrompt(string title, string text, IWindowContext parentWindowContext)
        {
            string yes = MainModel.Localization.CurrentLanguage.MessageBox.Yes;
            string no = MainModel.Localization.CurrentLanguage.MessageBox.No;
            return WindowManager.ShowPrompt(title, text, yes, no, parentWindowContext);
        }

    }
}
