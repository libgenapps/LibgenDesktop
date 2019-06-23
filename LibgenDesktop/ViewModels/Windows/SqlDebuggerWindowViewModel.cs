using System;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Localization.Localizators;

namespace LibgenDesktop.ViewModels.Windows
{
    internal class SqlDebuggerWindowViewModel : ViewModel
    {
        private readonly MainModel mainModel;
        private bool isSqlQueryTextBoxEnabled;
        private string result;
        private bool isCopyButtonEnabled;
        private bool isCloseButtonEnabled;
        private bool isQueryInProgress;

        public SqlDebuggerWindowViewModel(MainModel mainModel)
        {
            this.mainModel = mainModel;
            Events = new EventProvider();
            SqlQuery = String.Empty;
            isSqlQueryTextBoxEnabled = true;
            result = String.Empty;
            isCopyButtonEnabled = false;
            isCloseButtonEnabled = true;
            isQueryInProgress = false;
            Localization = mainModel.Localization.CurrentLanguage.SqlDebugger;
            WindowClosingCommand = new FuncCommand<bool>(WindowClosing);
            RunSqlCommand = new Command(RunSqlAsync);
            CopyResultCommand = new Command(CopyResultToClipboard);
            Events.RaiseEvent(ViewModelEvent.RegisteredEventId.FOCUS_SQL_QUERY_TEXT_BOX);
        }

        public SqlDebuggerWindowLocalizator Localization { get; }
        public EventProvider Events { get; }
        public string SqlQuery { get; set; }

        public bool IsSqlQueryTextBoxEnabled
        {
            get
            {
                return isSqlQueryTextBoxEnabled;
            }
            set
            {
                isSqlQueryTextBoxEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public string Result
        {
            get
            {
                return result;
            }
            private set
            {
                result = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsCopyButtonEnabled
        {
            get
            {
                return isCopyButtonEnabled;
            }
            private set
            {
                isCopyButtonEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsCloseButtonEnabled
        {
            get
            {
                return isCloseButtonEnabled;
            }
            private set
            {
                isCloseButtonEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public FuncCommand<bool> WindowClosingCommand { get; }
        public Command RunSqlCommand { get; }
        public Command CopyResultCommand { get; }

        private async void RunSqlAsync()
        {
            if (String.IsNullOrWhiteSpace(SqlQuery))
            {
                return;
            }
            isQueryInProgress = true;
            IsSqlQueryTextBoxEnabled = false;
            IsCopyButtonEnabled = false;
            IsCloseButtonEnabled = false;
            Result = "Executing the query...";
            try
            {
                Result = await mainModel.RunCustomSqlQuery(SqlQuery);
            }
            catch (Exception exception)
            {
                Result = exception.ToString();
            }
            IsSqlQueryTextBoxEnabled = true;
            IsCopyButtonEnabled = true;
            IsCloseButtonEnabled = true;
            isQueryInProgress = false;
        }

        private void CopyResultToClipboard()
        {
            WindowManager.SetClipboardText(Result ?? String.Empty);
        }

        private bool WindowClosing()
        {
            return !isQueryInProgress;
        }
    }
}
