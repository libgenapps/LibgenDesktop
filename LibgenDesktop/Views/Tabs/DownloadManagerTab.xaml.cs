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
                if (downloadManagerListBox.SelectedItems.Count > 0)
                {
                    downloadManagerListBox.ScrollIntoView(downloadManagerListBox.SelectedItems[0]);
                }
            }
        }
    }
}
