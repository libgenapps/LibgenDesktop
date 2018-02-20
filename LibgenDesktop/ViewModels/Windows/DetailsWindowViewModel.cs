using System;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.ViewModels.EventArguments;
using LibgenDesktop.ViewModels.Tabs;

namespace LibgenDesktop.ViewModels.Windows
{
    internal abstract class DetailsWindowViewModel<T> : LibgenWindowViewModel where T : LibgenObject
    {
        private readonly T libgenObject;
        private readonly bool modalWindow;
        private DetailsTabViewModel<T> tabViewModel;

        public DetailsWindowViewModel(MainModel mainModel, T libgenObject, bool modalWindow)
            : base(mainModel)
        {
            this.libgenObject = libgenObject;
            this.modalWindow = modalWindow;
            tabViewModel = null;
            WindowClosedCommand = new Command(WindowClosedHandler);
        }

        public string WindowTitle { get; protected set; }
        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }

        public DetailsTabViewModel<T> TabViewModel
        {
            get
            {
                if (tabViewModel == null)
                {
                    tabViewModel = CreateDetailsTabViewModel(MainModel, CurrentWindowContext, libgenObject, modalWindow);
                    tabViewModel.SelectDownloadRequested += SelectDownloadRequestedHandler;
                    tabViewModel.CloseTabRequested += CloseTabRequested;
                }
                return tabViewModel;
            }
        }

        public Command WindowClosedCommand { get; }

        public event EventHandler<SelectDownloadEventArgs> SelectDownloadRequested;
        public event EventHandler WindowClosed;

        protected abstract DetailsTabViewModel<T> CreateDetailsTabViewModel(MainModel mainModel, IWindowContext currentWindowContext, T libgenObject,
            bool modalWindow);

        protected virtual void OnWindowClosing()
        {
        }

        private void CloseTabRequested(object sender, EventArgs e)
        {
            if (modalWindow)
            {
                CurrentWindowContext.CloseDialog(false);
            }
            else
            {
                CurrentWindowContext.Close();
            }
        }

        private void SelectDownloadRequestedHandler(object sender, SelectDownloadEventArgs e)
        {
            SelectDownloadRequested?.Invoke(this, e);
        }

        private void WindowClosedHandler()
        {
            if (tabViewModel != null)
            {
                tabViewModel.CloseTabRequested -= CloseTabRequested;
                tabViewModel.SelectDownloadRequested -= SelectDownloadRequestedHandler;
            }
            OnWindowClosing();
            WindowClosed?.Invoke(this, EventArgs.Empty);
        }
    }
}
