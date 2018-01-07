using LibgenDesktop.Infrastructure;

namespace LibgenDesktop.Views.Tabs
{
    public partial class NonFictionSearchResultsTab : IEventListener
    {
        public NonFictionSearchResultsTab()
        {
            InitializeComponent();
        }

        public void OnViewModelEvent(ViewModelEvent viewModelEvent)
        {
            if (viewModelEvent.EventId == ViewModelEvent.RegisteredEventId.FOCUS_SEARCH_TEXT_BOX)
            {
                searchTextBox.Focus();
            }
        }
    }
}
