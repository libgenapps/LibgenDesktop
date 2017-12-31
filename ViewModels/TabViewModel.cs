using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;

namespace LibgenDesktop.ViewModels
{
    internal abstract class TabViewModel : ViewModel
    {
        private string title;

        protected TabViewModel(MainModel mainModel, IWindowContext mainWindowContext, string title)
        {
            MainModel = mainModel;
            MainWindowContext = mainWindowContext;
            this.title = title;
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

        public EventProvider Events { get; }

        protected MainModel MainModel { get; }
        protected IWindowContext MainWindowContext { get; }
    }
}
