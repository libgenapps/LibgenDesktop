using LibgenDesktop.Infrastructure;

namespace LibgenDesktop.Views
{
    public partial class MainWindow : IEventListener
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void OnViewModelEvent(ViewModelEvent viewModelEvent)
        {
        }
    }
}
