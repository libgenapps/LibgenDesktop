using System;
using System.Linq;
using System.Threading;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.ProgressArgs;
using LibgenDesktop.Models.Settings;
using LibgenDesktop.Models.Utils;

namespace LibgenDesktop.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        private readonly MainModel mainModel;
        private AsyncBookCollection books;
        private bool allowSqlDumpImport;
        private bool isInOfflineMode;
        private bool isInSearchMode;
        private double progressValue;
        private bool isProgressVisible;
        private bool isProgressIndeterminate;
        private bool isSearchindDisabled;
        private string statusText;

        public MainWindowViewModel()
        {
            mainModel = new MainModel();
            OpenBookDetailsCommand = new Command(param => OpenBookDetails(param as Book));
            ImportSqlDumpCommand = new Command(ImportSqlDump);
            ExitCommand = new Command(Exit);
            SearchCommand = new Command(Search);
            BookDataGridEnterKeyCommand = new Command(BookDataGridEnterKeyPressed);
            WindowClosedCommand = new Command(WindowClosed);
            Initialize();
        }

        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }
        public int WindowLeft { get; set; }
        public int WindowTop { get; set; }
        public bool IsWindowMaximized { get; set; }
        public int TitleColumnWidth { get; set; }
        public int AuthorsColumnWidth { get; set; }
        public int SeriesColumnWidth { get; set; }
        public int YearColumnWidth { get; set; }
        public int PublisherColumnWidth { get; set; }
        public int FormatColumnWidth { get; set; }
        public int FileSizeColumnWidth { get; set; }
        public int OcrColumnWidth { get; set; }
        public string SearchQuery { get; set; }
        public Book SelectedBook { get; set; }

        public AsyncBookCollection Books
        {
            get
            {
                return books;
            }
            private set
            {
                books = value;
                NotifyPropertyChanged();
            }
        }

        public bool AllowSqlDumpImport
        {
            get
            {
                return allowSqlDumpImport;
            }
            set
            {
                allowSqlDumpImport = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsInOfflineMode
        {
            get
            {
                return isInOfflineMode;
            }
            set
            {
                isInOfflineMode = value;
                NotifyPropertyChanged();
                mainModel.AppSettings.OfflineMode = value;
                mainModel.SaveSettings();
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

        public bool IsProgressVisible
        {
            get
            {
                return isProgressVisible;
            }
            private set
            {
                isProgressVisible = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsProgressIndeterminate
        {
            get
            {
                return isProgressIndeterminate;
            }
            set
            {
                isProgressIndeterminate = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsSearchindDisabled
        {
            get
            {
                return isSearchindDisabled;
            }
            private set
            {
                isSearchindDisabled = value;
                NotifyPropertyChanged();
            }
        }

        public string StatusText
        {
            get
            {
                return statusText;
            }
            private set
            {
                statusText = value;
                NotifyPropertyChanged();
            }
        }

        public Command OpenBookDetailsCommand { get; }
        public Command ImportSqlDumpCommand { get; }
        public Command ExitCommand { get; }
        public Command SearchCommand { get; }
        public Command BookDataGridEnterKeyCommand { get; }
        public Command WindowClosedCommand { get; }

        private async void Initialize()
        {
            AppSettings appSettings = mainModel.AppSettings;
            AppSettings.MainWindowSettings mainWindowSettings = appSettings.MainWindow;
            WindowWidth = mainWindowSettings.Width;
            WindowHeight = mainWindowSettings.Height;
            WindowLeft = mainWindowSettings.Left;
            WindowTop = mainWindowSettings.Top;
            IsWindowMaximized = mainWindowSettings.Maximized;
            AppSettings.ColumnSettings columnSettings = appSettings.Columns;
            TitleColumnWidth = columnSettings.TitleColumnWidth;
            AuthorsColumnWidth = columnSettings.AuthorsColumnWidth;
            SeriesColumnWidth = columnSettings.SeriesColumnWidth;
            YearColumnWidth = columnSettings.YearColumnWidth;
            PublisherColumnWidth = columnSettings.PublisherColumnWidth;
            FormatColumnWidth = columnSettings.FormatColumnWidth;
            FileSizeColumnWidth = columnSettings.FileSizeColumnWidth;
            OcrColumnWidth = columnSettings.OcrColumnWidth;
            isInOfflineMode = appSettings.OfflineMode;
            allowSqlDumpImport = false;
            SearchQuery = String.Empty;
            IsSearchindDisabled = false;
            books = mainModel.AllBooks;
            isInSearchMode = false;
            progressValue = 0;
            isProgressVisible = true;
            isProgressIndeterminate = false;
            statusText = "Загрузка книг...";
            Progress<LoadAllBooksProgress> loadAllBooksProgressHandler = new Progress<LoadAllBooksProgress>(HandleLoadAllBooksProgress);
            CancellationToken cancellationToken = new CancellationToken();
            try
            {
                await mainModel.LoadAllBooksAsync(loadAllBooksProgressHandler, cancellationToken);
            }
            catch (Exception exception)
            {
                ShowErrorWindow(exception);
            }
            InitialLoadCompleted();
        }

        private void HandleLoadAllBooksProgress(LoadAllBooksProgress loadAllBooksProgress)
        {
            if (!isInSearchMode)
            {
                IsProgressIndeterminate = false;
                if (!loadAllBooksProgress.IsFinished)
                {
                    IsProgressVisible = true;
                    StatusText = $"Загрузка книг (загружено { loadAllBooksProgress.BooksLoaded.ToString("N0", Formatters.ThousandsSeparatedNumberFormat)} из { loadAllBooksProgress.TotalBookCount.ToString("N0", Formatters.ThousandsSeparatedNumberFormat)})...";
                    ProgressValue = (double)loadAllBooksProgress.BooksLoaded / loadAllBooksProgress.TotalBookCount;
                }
            }
        }

        private void BookDataGridEnterKeyPressed()
        {
            OpenBookDetails(SelectedBook);
        }

        private void OpenBookDetails(Book book)
        {
            IWindowContext currentWindowContext = WindowManager.GetCreatedWindowContext(this);
            BookDetailsWindowViewModel bookDetailsWindowViewModel = new BookDetailsWindowViewModel(mainModel, book);
            IWindowContext bookDetailsWindowContext = WindowManager.CreateWindow(RegisteredWindows.WindowKey.BOOK_DETAILS_WINDOW, bookDetailsWindowViewModel, currentWindowContext);
            AppSettings.BookWindowSettings bookWindowSettings = mainModel.AppSettings.BookWindow;
            bookDetailsWindowContext.ShowDialog(bookWindowSettings.Width, bookWindowSettings.Height);
        }

        private void ImportSqlDump()
        {
            OpenFileDialogParameters selectSqlDumpFileDialogParameters = new OpenFileDialogParameters
            {
                DialogTitle = "Выбор SQL-дампа",
                Filter = "SQL-дампы (*.sql, *.zip, *.rar)|*.sql;*zip;*.rar|Все файлы (*.*)|*.*",
                Multiselect = false
            };
            OpenFileDialogResult selectSqlDumpFileDialogResult = WindowManager.ShowOpenFileDialog(selectSqlDumpFileDialogParameters);
            if (selectSqlDumpFileDialogResult.DialogResult)
            {
                StatusText = "Импорт из SQL-дампа...";
                IWindowContext currentWindowContext = WindowManager.GetCreatedWindowContext(this);
                SqlDumpImportWindowViewModel sqlDumpImportWindowViewModel = new SqlDumpImportWindowViewModel(mainModel, selectSqlDumpFileDialogResult.SelectedFilePaths.First());
                IWindowContext sqlDumpImportWindowContext = WindowManager.CreateWindow(RegisteredWindows.WindowKey.SQL_DUMP_IMPORT_WINDOW, sqlDumpImportWindowViewModel, currentWindowContext);
                sqlDumpImportWindowContext.ShowDialog();
                SetTotalBookNumberStatus();
            }
        }

        private async void Search()
        {
            string searchQuery = SearchQuery.Trim();
            if (String.IsNullOrEmpty(searchQuery))
            {
                Books = mainModel.AllBooks;
                SetTotalBookNumberStatus();
                IsSearchindDisabled = false;
                isInSearchMode = false;
            }
            else
            {
                isInSearchMode = true;
                IsProgressIndeterminate = true;
                IsProgressVisible = true;
                IsSearchindDisabled = true;
                statusText = "Поиск книг...";
                mainModel.ClearSearchResults();
                Books = mainModel.SearchResults;
                Progress<SearchBooksProgress> searchBooksProgressHandler = new Progress<SearchBooksProgress>(HandleSearchBooksProgress);
                CancellationToken cancellationToken = new CancellationToken();
                try
                {
                    await mainModel.SearchBooksAsync(searchQuery, searchBooksProgressHandler, cancellationToken);
                }
                catch (Exception exception)
                {
                    ShowErrorWindow(exception);
                }
                IsProgressVisible = false;
                IsSearchindDisabled = false;
                SetFoundBooksStatus();
            }
        }

        private void HandleSearchBooksProgress(SearchBooksProgress searchBooksProgress)
        {
            if (isInSearchMode)
            {
                if (!searchBooksProgress.IsFinished)
                {
                    IsProgressVisible = true;
                    IsProgressIndeterminate = true;
                    StatusText = $"Поиск книг (найдено { searchBooksProgress.BooksFound.ToString("N0", Formatters.ThousandsSeparatedNumberFormat)})...";
                }
            }
        }

        private void SetTotalBookNumberStatus()
        {
            StatusText = $"Всего книг: {Books.Count.ToString("N0", Formatters.ThousandsSeparatedNumberFormat)}";
        }

        private void SetFoundBooksStatus()
        {
            StatusText = $"Найдено книг: {Books.Count.ToString("N0", Formatters.ThousandsSeparatedNumberFormat)}";
        }

        private void InitialLoadCompleted()
        {
            UpdateAllowSqlDumpImport();
            if (!isInSearchMode)
            {
                IsProgressVisible = false;
                SetTotalBookNumberStatus();
            }
        }

        private void UpdateAllowSqlDumpImport()
        {
            AllowSqlDumpImport = mainModel.AllBooks.Count == 0;
        }

        private void Exit()
        {
            IWindowContext currentWindowContext = WindowManager.GetCreatedWindowContext(this);
            currentWindowContext.Close();
        }

        private void WindowClosed()
        {
            mainModel.AppSettings.MainWindow = new AppSettings.MainWindowSettings
            {
                Width = WindowWidth,
                Height = WindowHeight,
                Left = WindowLeft,
                Top = WindowTop,
                Maximized = IsWindowMaximized
            };
            mainModel.AppSettings.Columns = new AppSettings.ColumnSettings
            {
                TitleColumnWidth = TitleColumnWidth,
                AuthorsColumnWidth = AuthorsColumnWidth,
                SeriesColumnWidth = SeriesColumnWidth,
                YearColumnWidth = YearColumnWidth,
                PublisherColumnWidth = PublisherColumnWidth,
                FormatColumnWidth = FormatColumnWidth,
                FileSizeColumnWidth = FileSizeColumnWidth,
                OcrColumnWidth = OcrColumnWidth
            };
            mainModel.SaveSettings();
        }
    }
}
