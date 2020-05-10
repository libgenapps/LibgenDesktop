using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Localization.Localizators.Windows;
using LibgenDesktop.Models.Settings;
using static LibgenDesktop.Common.Constants;

namespace LibgenDesktop.ViewModels.Windows
{
    internal class SetupWizardProxySettingsWindowViewModel : LibgenWindowViewModel, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, string> errors;
        private SetupWizardProxySettingsWindowLocalizator localization;
        private string proxyAddress;
        private string proxyPort;
        private string proxyUserName;
        private string proxyPassword;
        private bool isOkButtonEnabled;

        public SetupWizardProxySettingsWindowViewModel(MainModel mainModel)
            : base(mainModel)
        {
            errors = new Dictionary<string, string>();
            localization = mainModel.Localization.CurrentLanguage.SetupWizardProxySettingsWindowLocalizator;
            mainModel.Localization.LanguageChanged += LocalizationLanguageChanged;
            OkCommand = new Command(OkButtonClick);
            Events = new EventProvider();
            Initialize();
        }

        public SetupWizardProxySettingsWindowLocalizator Localization
        {
            get
            {
                return localization;
            }
            set
            {
                localization = value;
                NotifyPropertyChanged();
            }
        }

        public string ProxyAddress
        {
            get
            {
                return proxyAddress;
            }
            set
            {
                proxyAddress = value;
                NotifyPropertyChanged();
                Validate();
            }
        }

        public string ProxyPort
        {
            get
            {
                return proxyPort;
            }
            set
            {
                proxyPort = value;
                NotifyPropertyChanged();
                Validate();
            }
        }
        public string ProxyUserName
        {
            get
            {
                return proxyUserName;
            }
            set
            {
                proxyUserName = value;
                NotifyPropertyChanged();
            }
        }

        public string ProxyPassword
        {
            get
            {
                return proxyPassword;
            }
            set
            {
                proxyPassword = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsOkButtonEnabled
        {
            get
            {
                return isOkButtonEnabled;
            }
            set
            {
                isOkButtonEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public bool HasErrors => errors.Any();

        public IEnumerable GetErrors(string propertyName)
        {
            if (errors.TryGetValue(propertyName, out string error))
            {
                yield return error;
            }
        }

        public Command OkCommand { get; }
        public EventProvider Events { get; }

        private int? ProxyPortValue
        {
            get
            {
                if (Int32.TryParse(ProxyPort, out int value))
                {
                    if (value >= MIN_PROXY_PORT && value <= MAX_PROXY_PORT)
                    {
                        return value;
                    }
                }
                return null;
            }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public void PopulateFieldsFromAppSettings()
        {
            AppSettings appSettings = MainModel.AppSettings;
            ProxyAddress = appSettings.Network.ProxyAddress;
            ProxyPort = appSettings.Network.ProxyPort?.ToString() ?? String.Empty;
            ProxyUserName = appSettings.Network.ProxyUserName;
            ProxyPassword = appSettings.Network.ProxyPassword;
            Validate();
        }

        public void SetFocus()
        {
            Events.RaiseEvent(ViewModelEvent.RegisteredEventId.FOCUS_SETUP_WIZARD_PROXY_SETTINGS_ADDRESS_TEXT_BOX);
        }

        private void Initialize()
        {
            proxyAddress = String.Empty;
            proxyPort = String.Empty;
            proxyUserName = String.Empty;
            proxyPassword = String.Empty;
            Validate();
        }

        private void Validate()
        {
            bool networkProxyAddressValid = !String.IsNullOrWhiteSpace(ProxyAddress);
            bool networkProxyPortValid = ProxyPortValue.HasValue;
            UpdateValidationState(nameof(ProxyAddress), networkProxyAddressValid, Localization.ProxyAddressRequired);
            UpdateValidationState(nameof(ProxyPort), networkProxyPortValid, Localization.GetProxyPortValidation(MIN_PROXY_PORT, MAX_PROXY_PORT));
            IsOkButtonEnabled = networkProxyAddressValid && networkProxyPortValid;
        }

        private void UpdateValidationState(string propertyName, bool isValid, string errorText)
        {
            if (isValid && errors.ContainsKey(propertyName))
            {
                errors.Remove(propertyName);
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
            else if (!isValid && !errors.ContainsKey(propertyName))
            {
                errors.Add(propertyName, errorText);
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        private void OkButtonClick()
        {
            AppSettings appSettings = MainModel.AppSettings;
            appSettings.Network.ProxyAddress = ProxyAddress;
            appSettings.Network.ProxyPort = ProxyPortValue;
            appSettings.Network.ProxyUserName = ProxyUserName ?? String.Empty;
            appSettings.Network.ProxyPassword = ProxyPassword ?? String.Empty;
            MainModel.SaveSettings();
            CurrentWindowContext.CloseDialog(true);
        }

        private void LocalizationLanguageChanged(object sender, EventArgs e)
        {
            Localization = MainModel.Localization.CurrentLanguage.SetupWizardProxySettingsWindowLocalizator;
        }
    }
}
