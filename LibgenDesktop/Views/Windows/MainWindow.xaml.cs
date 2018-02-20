using LibgenDesktop.Infrastructure;

namespace LibgenDesktop.Views.Windows
{
    public partial class MainWindow : IEventListener
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void OnViewModelEvent(ViewModelEvent viewModelEvent)
        {
            if (viewModelEvent.EventId == ViewModelEvent.RegisteredEventId.BRING_TO_FRONT)
            {
                Activate();
            }
        }
    }
}
