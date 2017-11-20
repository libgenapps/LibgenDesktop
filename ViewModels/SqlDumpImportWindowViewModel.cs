using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.ProgressArgs;
using LibgenDesktop.Models.Utils;

namespace LibgenDesktop.ViewModels
{
    internal class SqlDumpImportWindowViewModel : ViewModel
    {
        private readonly MainModel mainModel;
        private readonly string sqlDumpFilePath;
        private DateTime operationStartTime;
        private CancellationTokenSource cancellationTokenSource;
        private bool allowWindowClose;
        private string progressDescription;
        private double progressValue;
        private string status;
        private bool cancellationEnabled;

        public SqlDumpImportWindowViewModel(MainModel mainModel, string sqlDumpFilePath)
        {
            this.mainModel = mainModel;
            this.sqlDumpFilePath = sqlDumpFilePath;
            CancelCommand = new Command(CancelImport);
            WindowClosingCommand = new FuncCommand<bool>(WindowClosing);
            Initialize();
        }

        public string ProgressDescription
        {
            get
            {
                return progressDescription;
            }
            private set
            {
                progressDescription = value;
                NotifyPropertyChanged();
            }
        }

        public double ProgressValue
        {
            get
            {
                return progressValue;
            }
            private set
            {
                progressValue = value;
                NotifyPropertyChanged();
            }
        }

        public string Status
        {
            get
            {
                return status;
            }
            private set
            {
                status = value;
                NotifyPropertyChanged();
            }
        }

        public bool CancellationEnabled
        {
            get
            {
                return cancellationEnabled;
            }
            private set
            {
                cancellationEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public Command CancelCommand { get; }
        public FuncCommand<bool> WindowClosingCommand { get; }

        private async void Initialize()
        {
            operationStartTime = DateTime.Now;
            ProgressDescription = "Импорт из SQL-дампа...";
            UpdateStatus(0);
            cancellationEnabled = true;
            allowWindowClose = false;
            Progress<ImportSqlDumpProgress> importSqlDumpProgressHandler = new Progress<ImportSqlDumpProgress>(HandleImportSqlDumpProgress);
            cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            try
            {
                await mainModel.ImportSqlDumpAsync(sqlDumpFilePath, importSqlDumpProgressHandler, cancellationToken);
            }
            catch (Exception exception)
            {
                ShowErrorWindow(exception);
            }
            allowWindowClose = true;
            IWindowContext currentWindowContext = WindowManager.GetCreatedWindowContext(this);
            currentWindowContext.Close();
        }

        private void UpdateStatus(double completed)
        {
            DateTime now = DateTime.Now;
            TimeSpan elapsed = now - operationStartTime;
            string newStatus = $"Прошло {elapsed:mm\\:ss}";
            if (completed > 0.05)
            {
                TimeSpan remaining = TimeSpan.FromSeconds(elapsed.TotalSeconds / completed * (1 - completed));
                newStatus += $", осталось {remaining:mm\\:ss}";
            }
            Status = newStatus;
            ProgressValue = completed;
        }

        private void HandleImportSqlDumpProgress(ImportSqlDumpProgress importSqlDumpProgress)
        {
            ProgressDescription = $"Импорт из SQL-дампа... (импортировано {importSqlDumpProgress.BooksImported.ToString("N0", Formatters.ThousandsSeparatedNumberFormat)} книг)";
            UpdateStatus((double)importSqlDumpProgress.BytesParsed / importSqlDumpProgress.TotalBytes);
        }

        private void CancelImport()
        {
            CancellationEnabled = false;
            cancellationTokenSource.Cancel();
        }

        private bool WindowClosing()
        {
            if (!allowWindowClose)
            {
                CancelImport();
            }
            return allowWindowClose;
        }
    }
}
