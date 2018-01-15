using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using LibgenDesktop.Models.Database;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Import;
using LibgenDesktop.Models.JsonApi;
using LibgenDesktop.Models.ProgressArgs;
using LibgenDesktop.Models.Settings;
using LibgenDesktop.Models.SqlDump;
using static LibgenDesktop.Common.Constants;

namespace LibgenDesktop.Models
{
    internal class MainModel
    {
        internal enum DatabaseStatus
        {
            OPENED = 1,
            NOT_FOUND,
            NOT_SET,
            CORRUPTED
        }

        internal enum ImportSqlDumpResult
        {
            COMPLETED = 1,
            CANCELLED,
            DATA_NOT_FOUND
        }

        internal enum SynchronizationResult
        {
            COMPLETED = 1,
            CANCELLED
        }

        private const double SEARCH_PROGRESS_UPDATE_INTERVAL = 0.1;

        private readonly string settingsFilePath;
        private LocalDatabase localDatabase;

        public MainModel()
        {
            AppBinariesDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            AppDataDirectory = AppBinariesDirectory;
            settingsFilePath = Path.Combine(AppDataDirectory, APP_SETTINGS_FILE_NAME);
            if (File.Exists(settingsFilePath))
            {
                AppSettings = SettingsStorage.LoadSettings(settingsFilePath);
                IsInPortableMode = true;
            }
            else
            {
                try
                {
                    using (FileStream fileStream = File.Create(settingsFilePath))
                    {
                        fileStream.Close();
                    }
                    IsInPortableMode = true;
                }
                catch (UnauthorizedAccessException)
                {
                    IsInPortableMode = false;
                }
                if (IsInPortableMode)
                {
                    AppSettings = AppSettings.Default;
                    SaveSettings();
                }
                else
                {
                    AppDataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LibgenDesktop");
                    if (!Directory.Exists(AppDataDirectory))
                    {
                        Directory.CreateDirectory(AppDataDirectory);
                    }
                    settingsFilePath = Path.Combine(AppDataDirectory, APP_SETTINGS_FILE_NAME);
                    AppSettings = SettingsStorage.LoadSettings(settingsFilePath);
                }
            }
            OpenDatabase(AppSettings.DatabaseFileName);
            Mirrors = MirrorStorage.LoadMirrors(Path.Combine(AppBinariesDirectory, MIRRORS_FILE_NAME));
        }

        public AppSettings AppSettings { get; }
        public string AppBinariesDirectory { get; }
        public string AppDataDirectory { get; }
        public bool IsInPortableMode { get; }
        public DatabaseStatus LocalDatabaseStatus { get; private set; }
        public DatabaseMetadata DatabaseMetadata { get; private set; }
        public Mirrors Mirrors { get; }
        public int NonFictionBookCount { get; private set; }
        public int FictionBookCount { get; private set; }
        public int SciMagArticleCount { get; private set; }

        public Mirrors.MirrorConfiguration CurrentMirror
        {
            get
            {
                if (Mirrors != null && Mirrors.TryGetValue(AppSettings.Network.MirrorName, out Mirrors.MirrorConfiguration mirrorConfiguration))
                {
                    return mirrorConfiguration;
                }
                return null;
            }
        }

        public Task<ObservableCollection<NonFictionBook>> SearchNonFictionAsync(string searchQuery, IProgress<SearchProgress> progressHandler,
            CancellationToken cancellationToken)
        {
            return SearchItemsAsync(localDatabase.SearchNonFictionBooks, searchQuery, progressHandler, cancellationToken);
        }

        public Task<NonFictionBook> LoadNonFictionBookAsync(int bookId)
        {
            return LoadItemAsync(localDatabase.GetNonFictionBookById, bookId);
        }

        public Task<ObservableCollection<FictionBook>> SearchFictionAsync(string searchQuery, IProgress<SearchProgress> progressHandler,
            CancellationToken cancellationToken)
        {
            return SearchItemsAsync(localDatabase.SearchFictionBooks, searchQuery, progressHandler, cancellationToken);
        }

        public Task<FictionBook> LoadFictionBookAsync(int bookId)
        {
            return LoadItemAsync(localDatabase.GetFictionBookById, bookId);
        }

        public Task<ObservableCollection<SciMagArticle>> SearchSciMagAsync(string searchQuery, IProgress<SearchProgress> progressHandler,
            CancellationToken cancellationToken)
        {
            return SearchItemsAsync(localDatabase.SearchSciMagArticles, searchQuery, progressHandler, cancellationToken);
        }

        public Task<SciMagArticle> LoadSciMagArticleAsync(int articleId)
        {
            return LoadItemAsync(localDatabase.GetSciMagArticleById, articleId);
        }

        public Task<ImportSqlDumpResult> ImportSqlDumpAsync(string sqlDumpFilePath, IProgress<object> progressHandler, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                using (SqlDumpReader sqlDumpReader = new SqlDumpReader(sqlDumpFilePath))
                {
                    while (true)
                    {
                        bool tableFound = false;
                        while (sqlDumpReader.ReadLine())
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                return ImportSqlDumpResult.CANCELLED;
                            }
                            if (sqlDumpReader.CurrentLineCommand == SqlDumpReader.LineCommand.CREATE_TABLE)
                            {
                                tableFound = true;
                                break;
                            }
                            progressHandler.Report(new ImportSearchTableDefinitionProgress(sqlDumpReader.CurrentFilePosition, sqlDumpReader.FileSize));
                        }
                        if (!tableFound)
                        {
                            return ImportSqlDumpResult.DATA_NOT_FOUND;
                        }
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return ImportSqlDumpResult.CANCELLED;
                        }
                        SqlDumpReader.ParsedTableDefinition parsedTableDefinition = sqlDumpReader.ParseTableDefinition();
                        TableType tableType = DetectImportTableType(parsedTableDefinition);
                        if (tableType == TableType.UNKNOWN)
                        {
                            continue;
                        }
                        progressHandler.Report(new ImportTableDefinitionFoundProgress(tableType));
                        bool insertFound = false;
                        while (sqlDumpReader.ReadLine())
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                return ImportSqlDumpResult.CANCELLED;
                            }
                            if (sqlDumpReader.CurrentLineCommand == SqlDumpReader.LineCommand.INSERT)
                            {
                                insertFound = true;
                                break;
                            }
                        }
                        if (!insertFound)
                        {
                            return ImportSqlDumpResult.DATA_NOT_FOUND;
                        }
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return ImportSqlDumpResult.CANCELLED;
                        }
                        Importer importer;
                        switch (tableType)
                        {
                            case TableType.NON_FICTION:
                                if (NonFictionBookCount != 0)
                                {
                                    CheckAndCreateNonFictionIndexes(progressHandler, cancellationToken);
                                    if (cancellationToken.IsCancellationRequested)
                                    {
                                        return ImportSqlDumpResult.CANCELLED;
                                    }
                                }
                                importer = new NonFictionImporter(localDatabase, isUpdateMode: NonFictionBookCount != 0);
                                break;
                            case TableType.FICTION:
                                if (FictionBookCount != 0)
                                {
                                    CheckAndCreateFictionIndexes(progressHandler, cancellationToken);
                                    if (cancellationToken.IsCancellationRequested)
                                    {
                                        return ImportSqlDumpResult.CANCELLED;
                                    }
                                }
                                importer = new FictionImporter(localDatabase, isUpdateMode: FictionBookCount != 0);
                                break;
                            case TableType.SCI_MAG:
                                if (SciMagArticleCount != 0)
                                {
                                    CheckAndCreateSciMagIndexes(progressHandler, cancellationToken);
                                    if (cancellationToken.IsCancellationRequested)
                                    {
                                        return ImportSqlDumpResult.CANCELLED;
                                    }
                                }
                                importer = new SciMagImporter(localDatabase, isUpdateMode: SciMagArticleCount != 0);
                                break;
                            default:
                                throw new Exception($"Unknown table type: {tableType}.");
                        }
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return ImportSqlDumpResult.CANCELLED;
                        }
                        importer.Import(sqlDumpReader, progressHandler, cancellationToken, parsedTableDefinition);
                        switch (tableType)
                        {
                            case TableType.NON_FICTION:
                                UpdateNonFictionBookCount();
                                break;
                            case TableType.FICTION:
                                UpdateFictionBookCount();
                                break;
                            case TableType.SCI_MAG:
                                UpdateSciMagArticleCount();
                                break;
                        }
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return ImportSqlDumpResult.CANCELLED;
                        }
                        return ImportSqlDumpResult.COMPLETED;
                    }
                }
            });
        }

        public Task<SynchronizationResult> SynchronizeNonFiction(IProgress<object> progressHandler, CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                if (NonFictionBookCount == 0)
                {
                    throw new Exception("Non-fiction table must not be empty.");
                }
                CheckAndCreateNonFictionIndexes(progressHandler, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                {
                    return SynchronizationResult.CANCELLED;
                }
                NonFictionBook lastModifiedNonFictionBook = localDatabase.GetLastModifiedNonFictionBook();
                JsonApiClient jsonApiClient = new JsonApiClient(lastModifiedNonFictionBook.LastModifiedDateTime, lastModifiedNonFictionBook.LibgenId);
                List<NonFictionBook> downloadedBooks = new List<NonFictionBook>();
                progressHandler.Report(new JsonApiDownloadProgress(0));
                while (true)
                {
                    List<NonFictionBook> currentBatch;
                    try
                    {
                        currentBatch = await jsonApiClient.DownloadNextBatchAsync(cancellationToken);
                    }
                    catch (TaskCanceledException)
                    {
                        return SynchronizationResult.CANCELLED;
                    }
                    if (!currentBatch.Any())
                    {
                        break;
                    }
                    downloadedBooks.AddRange(currentBatch);
                    progressHandler.Report(new JsonApiDownloadProgress(downloadedBooks.Count));
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return SynchronizationResult.CANCELLED;
                    }
                }
                NonFictionImporter importer = new NonFictionImporter(localDatabase, lastModifiedNonFictionBook);
                importer.Import(downloadedBooks, progressHandler, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                {
                    return SynchronizationResult.CANCELLED;
                }
                return SynchronizationResult.COMPLETED;
            });
        }

        public void SaveSettings()
        {
            SettingsStorage.SaveSettings(AppSettings, settingsFilePath);
        }

        public string GetDatabaseNormalizedPath(string databaseFullPath)
        {
            if (IsInPortableMode)
            {
                if (databaseFullPath.ToLower().StartsWith(AppDataDirectory.ToLower()))
                {
                    return databaseFullPath.Substring(AppDataDirectory.Length + 1);
                }
                else
                {
                    return databaseFullPath;
                }
            }
            else
            {
                return databaseFullPath;
            }
        }

        public string GetDatabaseFullPath(string databaseNormalizedPath)
        {
            if (Path.IsPathRooted(databaseNormalizedPath))
            {
                return databaseNormalizedPath;
            }
            else
            {
                return Path.GetFullPath(Path.Combine(AppDataDirectory, databaseNormalizedPath));
            }
        }

        public bool OpenDatabase(string databaseFilePath)
        {
            if (localDatabase != null)
            {
                localDatabase.Dispose();
                localDatabase = null;
            }
            if (!String.IsNullOrWhiteSpace(databaseFilePath))
            {
                if (File.Exists(databaseFilePath))
                {
                    try
                    {
                        localDatabase = LocalDatabase.OpenDatabase(databaseFilePath);
                        if (!localDatabase.CheckIfMetadataExists())
                        {
                            LocalDatabaseStatus = DatabaseStatus.CORRUPTED;
                            return false;
                        }
                        DatabaseMetadata = localDatabase.GetMetadata();
                        if (DatabaseMetadata.Version != CURRENT_DATABASE_VERSION)
                        {
                            LocalDatabaseStatus = DatabaseStatus.CORRUPTED;
                            return false;
                        }
                        UpdateNonFictionBookCount();
                        UpdateFictionBookCount();
                        UpdateSciMagArticleCount();
                    }
                    catch
                    {
                        LocalDatabaseStatus = DatabaseStatus.CORRUPTED;
                        return false;
                    }
                    LocalDatabaseStatus = DatabaseStatus.OPENED;
                    return true;
                }
                else
                {
                    LocalDatabaseStatus = DatabaseStatus.NOT_FOUND;
                }
            }
            else
            {
                LocalDatabaseStatus = DatabaseStatus.NOT_SET;
            }
            return false;
        }

        public bool CreateDatabase(string databaseFilePath)
        {
            if (localDatabase != null)
            {
                localDatabase.Dispose();
                localDatabase = null;
            }
            try
            {
                localDatabase = LocalDatabase.CreateDatabase(databaseFilePath);
                localDatabase.CreateMetadataTable();
                localDatabase.CreateNonFictionTables();
                localDatabase.CreateFictionTables();
                localDatabase.CreateSciMagTables();
                DatabaseMetadata databaseMetadata = new DatabaseMetadata
                {
                    Version = CURRENT_DATABASE_VERSION
                };
                localDatabase.AddMetadata(databaseMetadata);
                NonFictionBookCount = 0;
                FictionBookCount = 0;
                SciMagArticleCount = 0;
                return true;
            }
            catch
            {
                LocalDatabaseStatus = DatabaseStatus.CORRUPTED;
                return false;
            }
        }

        private Task<ObservableCollection<T>> SearchItemsAsync<T>(Func<string, int?, IEnumerable<T>> searchFunction, string searchQuery,
            IProgress<SearchProgress> progressHandler, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                int? resultLimit = null;
                if (AppSettings.Search.LimitResults)
                {
                    resultLimit = AppSettings.Search.MaximumResultCount;
                }
                ObservableCollection<T> result = new ObservableCollection<T>();
                DateTime lastUpdateDateTime = DateTime.Now;
                foreach (T item in searchFunction(searchQuery, resultLimit))
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return null;
                    }
                    result.Add(item);
                    DateTime now = DateTime.Now;
                    if ((now - lastUpdateDateTime).TotalSeconds > SEARCH_PROGRESS_UPDATE_INTERVAL)
                    {
                        progressHandler.Report(new SearchProgress(result.Count));
                        lastUpdateDateTime = now;
                    }
                }
                progressHandler.Report(new SearchProgress(result.Count));
                return result;
            });
        }

        private Task<T> LoadItemAsync<T>(Func<int, T> loadFunction, int itemId)
        {
            return Task.Run(() =>
            {
                return loadFunction(itemId);
            });
        }

        private TableType DetectImportTableType(SqlDumpReader.ParsedTableDefinition parsedTableDefinition)
        {
            if (TableDefinitions.AllTables.TryGetValue(parsedTableDefinition.TableName, out TableDefinition tableDefinition))
            {
                foreach (SqlDumpReader.ParsedColumnDefinition parsedColumnDefinition in parsedTableDefinition.Columns)
                {
                    if (tableDefinition.Columns.TryGetValue(parsedColumnDefinition.ColumnName.ToLower(), out ColumnDefinition columnDefinition))
                    {
                        if (columnDefinition.ColumnType == parsedColumnDefinition.ColumnType)
                        {
                            continue;
                        }
                    }
                    return TableType.UNKNOWN;
                }
                return tableDefinition.TableType;
            }
            return TableType.UNKNOWN;
        }

        private void CheckAndCreateNonFictionIndexes(IProgress<object> progressHandler, CancellationToken cancellationToken)
        {
            List<string> nonFictionIndexes = localDatabase.GetNonFictionIndexList();
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            CheckAndCreateIndex(nonFictionIndexes, SqlScripts.NON_FICTION_INDEX_PREFIX, "LastModifiedDateTime", progressHandler,
                localDatabase.CreateNonFictionLastModifiedDateTimeIndex);
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            CheckAndCreateIndex(nonFictionIndexes, SqlScripts.NON_FICTION_INDEX_PREFIX, "LibgenId", progressHandler,
                localDatabase.CreateNonFictionLibgenIdIndex);
        }

        private void CheckAndCreateFictionIndexes(IProgress<object> progressHandler, CancellationToken cancellationToken)
        {
            List<string> fictionIndexes = localDatabase.GetFictionIndexList();
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            CheckAndCreateIndex(fictionIndexes, SqlScripts.FICTION_INDEX_PREFIX, "LastModifiedDateTime", progressHandler,
                localDatabase.CreateFictionLastModifiedDateTimeIndex);
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            CheckAndCreateIndex(fictionIndexes, SqlScripts.FICTION_INDEX_PREFIX, "LibgenId", progressHandler, localDatabase.CreateFictionLibgenIdIndex);
        }

        private void CheckAndCreateSciMagIndexes(IProgress<object> progressHandler, CancellationToken cancellationToken)
        {
            List<string> sciMagIndexes = localDatabase.GetSciMagIndexList();
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            CheckAndCreateIndex(sciMagIndexes, SqlScripts.SCIMAG_INDEX_PREFIX, "AddedDateTime", progressHandler, localDatabase.CreateSciMagAddedDateTimeIndex);
        }

        private void CheckAndCreateIndex(List<string> existingIndexes, string prefix, string fieldName, IProgress<object> progressHandler,
            Action createIndexAction)
        {
            if (!existingIndexes.Contains(prefix + fieldName))
            {
                progressHandler.Report(new ImportCreateIndexProgress(fieldName));
                createIndexAction();
            }
        }

        private void UpdateNonFictionBookCount()
        {
            NonFictionBookCount = localDatabase.CountNonFictionBooks();
        }

        private void UpdateFictionBookCount()
        {
            FictionBookCount = localDatabase.CountFictionBooks();
        }

        private void UpdateSciMagArticleCount()
        {
            SciMagArticleCount = localDatabase.CountSciMagArticles();
        }
    }
}
