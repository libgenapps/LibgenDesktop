using System;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Download;
using LibgenDesktop.Models.Localization;
using LibgenDesktop.Models.Localization.Localizators.Windows;

namespace LibgenDesktop.ViewModels.SetupSteps
{
    internal abstract partial class SetupStepViewModel : ContainerViewModel
    {
        internal enum SetupStage
        {
            DOWNLOADING_DUMPS = 1,
            CREATING_DATABASE = 2,
            IMPORTING_DUMPS = 3
        }

        internal class NavigationButtonVisibilityChangeRequestEventArgs : EventArgs
        {
            public NavigationButtonVisibilityChangeRequestEventArgs(bool isVisible)
            {
                IsVisible = isVisible;
            }

            public bool IsVisible { get; }
        }

        internal class PageNavigationRequestEventArgs : EventArgs
        {
            public PageNavigationRequestEventArgs(SetupWizardStep newPageStep)
            {
                NewPageStep = newPageStep;
            }

            public SetupWizardStep NewPageStep { get; }
        }

        private const int TOTAL_SETUP_STAGE_COUNT = 3;

        private readonly Func<IWindowContext> setupWizardWindowContextProxy;
        private bool isPageVisible;

        protected SetupStepViewModel(MainModel mainModel, Func<IWindowContext> setupWizardWindowContextProxy, SetupWizardWindowLocalizator windowLocalization,
            SharedSetupContext sharedSetupContext, SetupWizardStep step)
            : base(mainModel)
        {
            this.setupWizardWindowContextProxy = setupWizardWindowContextProxy;
            isPageVisible = false;
            WindowLocalization = windowLocalization;
            Step = step;
            SharedSetupContext = sharedSetupContext;
            mainModel.Localization.LanguageChanged += LocalizationLanguageChanged;
        }

        public bool IsPageVisible
        {
            get
            {
                return isPageVisible;
            }
            set
            {
                isPageVisible = value;
                NotifyPropertyChanged();
            }
        }

        public SetupWizardWindowLocalizator WindowLocalization { get; private set; }
        public SetupWizardStep Step { get; }

        public virtual bool IsBackButtonVisible => true;
        public virtual bool IsNextButtonVisible => true;
        public virtual bool IsFinishButtonVisible => false;

        protected SharedSetupContext SharedSetupContext { get; }

        public event EventHandler<NavigationButtonVisibilityChangeRequestEventArgs> BackButtonVisibilityChangeRequested;
        public event EventHandler<NavigationButtonVisibilityChangeRequestEventArgs> NextButtonVisibilityChangeRequested;
        public event EventHandler<NavigationButtonVisibilityChangeRequestEventArgs> CancelButtonVisibilityChangeRequested;
        public event EventHandler<NavigationButtonVisibilityChangeRequestEventArgs> CloseButtonVisibilityChangeRequested;
        public event EventHandler<PageNavigationRequestEventArgs> PageNavigationRequested;

        public virtual void OnPageEnter()
        {
            IsPageVisible = true;
        }

        public virtual void OnBackButtonClick()
        {
            HidePage();
        }

        public virtual void OnNextButtonClick()
        {
            HidePage();
        }

        public virtual void OnFinishButtonClick()
        {
        }

        public void ShowPage()
        {
            IsPageVisible = true;
        }

        public void HidePage()
        {
            IsPageVisible = false;
        }

        protected IWindowContext SetupWizardWindowContext => setupWizardWindowContextProxy();

        protected abstract void UpdateLocalization(SetupWizardWindowLocalizator windowLocalization);

        protected void ShowMessage(string title, string text)
        {
            ShowMessage(title, text, SetupWizardWindowContext);
        }

        protected bool ShowPrompt(string title, string text)
        {
            return ShowPrompt(title, text, SetupWizardWindowContext);
        }

        protected void HideBackButton()
        {
            BackButtonVisibilityChangeRequested?.Invoke(this, new NavigationButtonVisibilityChangeRequestEventArgs(false));
        }

        protected void ShowBackButton()
        {
            BackButtonVisibilityChangeRequested?.Invoke(this, new NavigationButtonVisibilityChangeRequestEventArgs(true));
        }

        protected void HideNextButton()
        {
            NextButtonVisibilityChangeRequested?.Invoke(this, new NavigationButtonVisibilityChangeRequestEventArgs(false));
        }

        protected void ShowNextButton()
        {
            NextButtonVisibilityChangeRequested?.Invoke(this, new NavigationButtonVisibilityChangeRequestEventArgs(true));
        }

        protected void HideCancelButton()
        {
            CancelButtonVisibilityChangeRequested?.Invoke(this, new NavigationButtonVisibilityChangeRequestEventArgs(false));
        }

        protected void ShowCancelButton()
        {
            CancelButtonVisibilityChangeRequested?.Invoke(this, new NavigationButtonVisibilityChangeRequestEventArgs(true));
        }

        protected void HideCloseButton()
        {
            CloseButtonVisibilityChangeRequested?.Invoke(this, new NavigationButtonVisibilityChangeRequestEventArgs(false));
        }

        protected void ShowCloseButton()
        {
            CloseButtonVisibilityChangeRequested?.Invoke(this, new NavigationButtonVisibilityChangeRequestEventArgs(true));
        }

        protected string GetHeaderString(SetupStage setupStage)
        {
            string header;
            switch (setupStage)
            {
                case SetupStage.DOWNLOADING_DUMPS:
                    header = WindowLocalization.DownloadingDumpsStepHeader;
                    break;
                case SetupStage.CREATING_DATABASE:
                    header = WindowLocalization.CreatingDatabaseStepHeader;
                    break;
                case SetupStage.IMPORTING_DUMPS:
                    header = WindowLocalization.ImportingDumpsStepHeader;
                    break;
                default:
                    throw new Exception($"Unexpected setup stage: {setupStage}.");
            }
            return WindowLocalization.GetStepHeader((int)setupStage, TOTAL_SETUP_STAGE_COUNT, header);
        }

        protected void MoveToPage(SetupWizardStep newPageStep)
        {
            HidePage();
            PageNavigationRequested?.Invoke(this, new PageNavigationRequestEventArgs(newPageStep));
        }

        protected string RoundedSizeUnitToString(LibgenDumpDownloader.RoundedSizeUnit roundedSizeUnit)
        {
            LanguageFormatter languageFormatter = MainModel.Localization.CurrentLanguage.Formatter;
            switch (roundedSizeUnit)
            {
                case LibgenDumpDownloader.RoundedSizeUnit.BYTES:
                    return languageFormatter.BytePostfix;
                case LibgenDumpDownloader.RoundedSizeUnit.KILOBYTES:
                    return languageFormatter.KilobytePostfix;
                case LibgenDumpDownloader.RoundedSizeUnit.MEGABYTES:
                    return languageFormatter.MegabytePostfix;
                case LibgenDumpDownloader.RoundedSizeUnit.GIGABYTES:
                    return languageFormatter.GigabytePostfix;
                case LibgenDumpDownloader.RoundedSizeUnit.TERABYTES:
                    return languageFormatter.TerabytePostfix;
                default:
                    throw new Exception($"Unknown rounded size unit: {roundedSizeUnit}.");
            }
        }

        private void LocalizationLanguageChanged(object sender, EventArgs e)
        {
            WindowLocalization = MainModel.Localization.CurrentLanguage.SetupWizardWindow;
            UpdateLocalization(WindowLocalization);
        }
    }
}
