using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;

namespace LibgenDesktop.ViewModels
{
    internal abstract class TabViewModel : ViewModel
    {
        private string title;

        protected TabViewModel(MainModel mainModel, IWindowContext parentWindowContext, string title)
        {
            MainModel = mainModel;
            ParentWindowContext = parentWindowContext;
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
        protected IWindowContext ParentWindowContext { get; }
    }
}
