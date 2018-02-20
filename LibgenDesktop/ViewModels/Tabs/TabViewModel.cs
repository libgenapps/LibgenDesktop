using System;
using System.Threading;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;

namespace LibgenDesktop.ViewModels.Tabs
{
    internal abstract class TabViewModel : ContainerViewModel
    {
        private readonly SynchronizationContext synchronizationContext;
        private string title;

        protected TabViewModel(MainModel mainModel, IWindowContext parentWindowContext, string title)
            : base(mainModel)
        {
            synchronizationContext = SynchronizationContext.Current;
            ParentWindowContext = parentWindowContext;
            this.title = title;
            RequestCloseCommand = new Command(RequestCloseTab);
            Events = new EventProvider();
        }

        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
                NotifyPropertyChanged();
            }
        }

        public Command RequestCloseCommand { get; }
        public EventProvider Events { get; }

        public event EventHandler CloseTabRequested;

        protected IWindowContext ParentWindowContext { get; }

        public virtual void HandleTabClosing()
        {
        }

        protected void ShowMessage(string title, string text)
        {
            ShowMessage(title, text, ParentWindowContext);
        }

        protected bool ShowPrompt(string title, string text)
        {
            return ShowPrompt(title, text, ParentWindowContext);
        }

        protected void ExecuteInUiThread(Action action)
        {
            synchronizationContext.Post(state => action(), null);
        }

        private void RequestCloseTab()
        {
            CloseTabRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
