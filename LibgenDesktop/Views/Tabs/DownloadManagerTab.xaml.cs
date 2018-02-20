using LibgenDesktop.Infrastructure;

namespace LibgenDesktop.Views.Tabs
{
    public partial class DownloadManagerTab : IEventListener
    {
        public DownloadManagerTab()
        {
            InitializeComponent();
        }

        public void OnViewModelEvent(ViewModelEvent viewModelEvent)
        {
            if (viewModelEvent.EventId == ViewModelEvent.RegisteredEventId.SCROLL_TO_SELECTION)
            {
                if (downloaderListBox.SelectedItems.Count > 0)
                {
                    downloaderListBox.ScrollIntoView(downloaderListBox.SelectedItems[0]);
                }
            }
        }
    }
}
