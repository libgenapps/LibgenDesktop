using System.Windows.Threading;
using LibgenDesktop.Infrastructure;

namespace LibgenDesktop.Views.Windows
{
    public partial class SqlDebuggerWindow : IEventListener
    {
        public SqlDebuggerWindow()
        {
            InitializeComponent();
        }

        public void OnViewModelEvent(ViewModelEvent viewModelEvent)
        {
            if (viewModelEvent.EventId == ViewModelEvent.RegisteredEventId.FOCUS_SQL_QUERY_TEXT_BOX)
            {
                Dispatcher.InvokeAsync(() =>
                {
                    sqlQueryTextBox.Focus();
                }, DispatcherPriority.ApplicationIdle);
            }
        }
    }
}
