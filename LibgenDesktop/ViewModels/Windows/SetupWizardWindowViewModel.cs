using System;
using System.Collections.Generic;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Localization.Localizators.Windows;
using LibgenDesktop.ViewModels.SetupSteps;

namespace LibgenDesktop.ViewModels.Windows
{
    internal class SetupWizardWindowViewModel : LibgenWindowViewModel
    {
        private readonly Dictionary<SetupWizardStep, SetupStepViewModel> pages;
        private readonly SharedSetupContext sharedSetupContext;
        private SetupStepViewModel currentPage;
        private bool isBackButtonVisible;
        private bool isNextButtonVisible;
        private bool isFinishButtonVisible;
        private bool isCancelButtonVisible;
        private bool isWindowCloseButtonVisible;

        public SetupWizardWindowViewModel(MainModel mainModel)
            : base(mainModel)
        {
            Localization = mainModel.Localization.CurrentLanguage.SetupWizardWindow;
            currentPage = null;
            isBackButtonVisible = false;
            isNextButtonVisible = false;
            isFinishButtonVisible = false;
            isCancelButtonVisible = true;
            isWindowCloseButtonVisible = true;
            pages = new Dictionary<SetupWizardStep, SetupStepViewModel>();
            sharedSetupContext = new SharedSetupContext();
            LanguagePageViewModel = RegisterPage<LanguagePageViewModel>();
            SetupModePageViewModel = RegisterPage<SetupModePageViewModel>();
            DatabaseOperationPageViewModel = RegisterPage<DatabaseOperationPageViewModel>();
            StepListPageViewModel = RegisterPage<StepListPageViewModel>();
            DownloadModePageViewModel = RegisterPage<DownloadModePageViewModel>();
            DownloadDumpInfoPageViewModel = RegisterPage<DownloadDumpInfoPageViewModel>();
            CollectionsPageViewModel = RegisterPage<CollectionsPageViewModel>();
            DownloadDumpsPageViewModel = RegisterPage<DownloadDumpsPageViewModel>();
            DownloadDumpLinksPageViewModel = RegisterPage<DownloadDumpLinksPageViewModel>();
            CreateDatabasePageViewModel = RegisterPage<CreateDatabasePageViewModel>();
            ImportDumpsPageViewModel = RegisterPage<ImportDumpsPageViewModel>();
            ConfirmationPageViewModel = RegisterPage<ConfirmationPageViewModel>();
            BackCommand = new Command(BackButtonClick);
            NextCommand = new Command(NextButtonClick);
            FinishCommand = new Command(FinishButtonClick);
            WindowClosingCommand = new FuncCommand<bool?, bool>(WindowClosing);
            MainModel.Localization.LanguageChanged += LocalizationLanguageChanged;
            MoveToPage(LanguagePageViewModel.Step);
        }

        public SetupWizardWindowLocalizator Localization { get; private set; }

        public bool IsBackButtonVisible
        {
            get
            {
                return isBackButtonVisible;
            }
            set
            {
                isBackButtonVisible = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsNextButtonVisible
        {
            get
            {
                return isNextButtonVisible;
            }
            set
            {
                isNextButtonVisible = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsFinishButtonVisible
        {
            get
            {
                return isFinishButtonVisible;
            }
            set
            {
                isFinishButtonVisible = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsCancelButtonVisible
        {
            get
            {
                return isCancelButtonVisible;
            }
            set
            {
                isCancelButtonVisible = value;
                NotifyPropertyChanged();
            }
        }

        public LanguagePageViewModel LanguagePageViewModel { get; }
        public SetupModePageViewModel SetupModePageViewModel { get; }
        public DatabaseOperationPageViewModel DatabaseOperationPageViewModel { get; }
        public StepListPageViewModel StepListPageViewModel { get; }
        public DownloadModePageViewModel DownloadModePageViewModel { get; }
        public DownloadDumpInfoPageViewModel DownloadDumpInfoPageViewModel { get; }
        public CollectionsPageViewModel CollectionsPageViewModel { get; }
        public DownloadDumpsPageViewModel DownloadDumpsPageViewModel { get; }
        public DownloadDumpLinksPageViewModel DownloadDumpLinksPageViewModel { get; }
        public CreateDatabasePageViewModel CreateDatabasePageViewModel { get; }
        public ImportDumpsPageViewModel ImportDumpsPageViewModel { get; }
        public ConfirmationPageViewModel ConfirmationPageViewModel { get; }

        public Command BackCommand { get; }
        public Command NextCommand { get; }
        public Command FinishCommand { get; }
        public Command CancelCommand { get; }
        public FuncCommand<bool?, bool> WindowClosingCommand { get; }

        private Func<IWindowContext> CurrentWindowContextProxy => () => CurrentWindowContext;

        private T RegisterPage<T>() where T : SetupStepViewModel
        {
            T result = Activator.CreateInstance(typeof(T), MainModel, CurrentWindowContextProxy, Localization, sharedSetupContext) as T;
            result.BackButtonVisibilityChangeRequested += (sender, e) => IsBackButtonVisible = e.IsVisible;
            result.NextButtonVisibilityChangeRequested += (sender, e) => IsNextButtonVisible = e.IsVisible;
            result.CancelButtonVisibilityChangeRequested += (sender, e) => IsCancelButtonVisible = e.IsVisible;
            result.CloseButtonVisibilityChangeRequested += (sender, e) =>
            {
                if (e.IsVisible)
                {
                    ShowWindowCloseButton();
                }
                else
                {
                    HideWindowCloseButton();
                }
            };
            result.PageNavigationRequested += (sender, e) => MoveToPage(e.NewPageStep);
            pages.Add(result.Step, result);
            return result;
        }

        private void BackButtonClick()
        {
            currentPage.OnBackButtonClick();
        }

        private void NextButtonClick()
        {
            currentPage.OnNextButtonClick();
        }

        private void FinishButtonClick()
        {
            currentPage.OnFinishButtonClick();
            CurrentWindowContext.CloseDialog(true);
        }

        private void ShowWindowCloseButton()
        {
            if (!isWindowCloseButtonVisible)
            {
                CurrentWindowContext.AddWindowCloseButton();
                isWindowCloseButtonVisible = true;
            }
        }

        private void HideWindowCloseButton()
        {
            if (isWindowCloseButtonVisible)
            {
                CurrentWindowContext.RemoveWindowCloseButton();
                isWindowCloseButtonVisible = false;
            }
        }

        private void MoveToPage(SetupWizardStep pageStep)
        {
            currentPage = pages[pageStep];
            IsBackButtonVisible = currentPage.IsBackButtonVisible;
            IsNextButtonVisible = currentPage.IsNextButtonVisible;
            IsFinishButtonVisible = currentPage.IsFinishButtonVisible;
            currentPage.OnPageEnter();
        }

        private void LocalizationLanguageChanged(object sender, EventArgs e)
        {
            Localization = MainModel.Localization.CurrentLanguage.SetupWizardWindow;
            NotifyPropertyChanged(nameof(Localization));
        }

        private bool WindowClosing(bool? dialogResult)
        {
            return (dialogResult == true) || (isWindowCloseButtonVisible && ShowPrompt(Localization.ExitSetupTitle, Localization.ExitSetupText));
        }
    }
}
