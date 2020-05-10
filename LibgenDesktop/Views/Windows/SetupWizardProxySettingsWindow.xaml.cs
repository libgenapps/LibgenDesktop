using LibgenDesktop.Infrastructure;

namespace LibgenDesktop.Views.Windows
{
    public partial class SetupWizardProxySettingsWindow : IEventListener
    {
        public SetupWizardProxySettingsWindow()
        {
            InitializeComponent();
        }

        public void OnViewModelEvent(ViewModelEvent viewModelEvent)
        {
            if (viewModelEvent.EventId == ViewModelEvent.RegisteredEventId.FOCUS_SETUP_WIZARD_PROXY_SETTINGS_ADDRESS_TEXT_BOX)
            {
                proxyAddressTextBox.Focus();
            }
        }
    }
}
