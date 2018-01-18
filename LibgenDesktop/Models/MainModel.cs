using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LibgenDesktop.Common;
using LibgenDesktop.Models.Database;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Import;
using LibgenDesktop.Models.JsonApi;
using LibgenDesktop.Models.ProgressArgs;
using LibgenDesktop.Models.Settings;
using LibgenDesktop.Models.SqlDump;
using static LibgenDesktop.Common.Constants;
using Environment = LibgenDesktop.Common.Environment;

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

        private LocalDatabase localDatabase;

        public MainModel()
        {
            AppSettings = SettingsStorage.LoadSettings(Environment.AppSettingsFilePath);
            if (AppSettings.Advanced.LoggingEnabled)
            {
                EnableLogging();
            }
            Mirrors = MirrorStorage.LoadMirrors(Environment.MirrorsFilePath);
            ValidateAndCorrectSelectedMirrors();
            CreateNewHttpClient();
            OpenDatabase(AppSettings.DatabaseFileName);
        }

        public AppSettings AppSettings { get; }
        public DatabaseStatus LocalDatabaseStatus { get; private set; }
        public DatabaseMetadata DatabaseMetadata { get; private set; }
        public Mirrors Mirrors { get; }
        public HttpClient HttpClient { get; private set; }
        public int NonFictionBookCount { get; private set; }
        public int FictionBookCount { get; private set; }
        public int SciMagArticleCount { get; private set; }

        public Task<ObservableCollection<NonFictionBook>> SearchNonFictionAsync(string searchQuery, IProgress<SearchProgress> progressHandler,
            CancellationToken cancellationToken)
        {
            Logger.Debug($@"Search query = ""{searchQuery}"".");
            return SearchItemsAsync(localDatabase.SearchNonFictionBooks, searchQuery, progressHandler, cancellationToken);
        }

        public Task<NonFictionBook> LoadNonFictionBookAsync(int bookId)
        {
            Logger.Debug($"Loading non-fiction book with ID = {bookId}.");
            return LoadItemAsync(localDatabase.GetNonFictionBookById, bookId);
        }

        public Task<ObservableCollection<FictionBook>> SearchFictionAsync(string searchQuery, IProgress<SearchProgress> progressHandler,
            CancellationToken cancellationToken)
        {
            Logger.Debug($@"Search query = ""{searchQuery}"".");
            return SearchItemsAsync(localDatabase.SearchFictionBooks, searchQuery, progressHandler, cancellationToken);
        }

        public Task<FictionBook> LoadFictionBookAsync(int bookId)
        {
            Logger.Debug($"Loading fiction book with ID = {bookId}.");
            return LoadItemAsync(localDatabase.GetFictionBookById, bookId);
        }

        public Task<ObservableCollection<SciMagArticle>> SearchSciMagAsync(string searchQuery, IProgress<SearchProgress> progressHandler,
            CancellationToken cancellationToken)
        {
            Logger.Debug($@"Search query = ""{searchQuery}"".");
            return SearchItemsAsync(localDatabase.SearchSciMagArticles, searchQuery, progressHandler, cancellationToken);
        }

        public Task<SciMagArticle> LoadSciMagArticleAsync(int articleId)
        {
            Logger.Debug($"Loading article with ID = {articleId}.");
            return LoadItemAsync(localDatabase.GetSciMagArticleById, articleId);
        }

        public Task<ImportSqlDumpResult> ImportSqlDumpAsync(string sqlDumpFilePath, IProgress<object> progressHandler, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                Logger.Debug("SQL dump import has started.");
                using (SqlDumpReader sqlDumpReader = new SqlDumpReader(sqlDumpFilePath))
                {
                    while (true)
                    {
                        bool tableFound = false;
                        while (sqlDumpReader.ReadLine())
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                Logger.Debug("SQL dump import has been cancelled.");
                                return ImportSqlDumpResult.CANCELLED;
                            }
                            if (sqlDumpReader.CurrentLineCommand == SqlDumpReader.LineCommand.CREATE_TABLE)
                            {
                                Logger.Debug("CREATE TABLE statement found.");
                                tableFound = true;
                                break;
                            }
                            progressHandler.Report(new ImportSearchTableDefinitionProgress(sqlDumpReader.CurrentFilePosition, sqlDumpReader.FileSize));
                        }
                        if (!tableFound)
                        {
                            Logger.Debug("CREATE TABLE statement was not found.");
                            return ImportSqlDumpResult.DATA_NOT_FOUND;
                        }
                        if (cancellationToken.IsCancellationRequested)
                        {
                            Logger.Debug("SQL dump import has been cancelled.");
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
                                Logger.Debug("SQL dump import has been cancelled.");
                                return ImportSqlDumpResult.CANCELLED;
                            }
                            if (sqlDumpReader.CurrentLineCommand == SqlDumpReader.LineCommand.INSERT)
                            {
                                Logger.Debug("INSERT statement found.");
                                insertFound = true;
                                break;
                            }
                        }
                        if (!insertFound)
                        {
                            Logger.Debug("INSERT statement was not found.");
                            return ImportSqlDumpResult.DATA_NOT_FOUND;
                        }
                        if (cancellationToken.IsCancellationRequested)
                        {
                            Logger.Debug("SQL dump import has been cancelled.");
                            return ImportSqlDumpResult.CANCELLED;
                        }
                        Logger.Debug($"Table type is {tableType}.");
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
                            Logger.Debug("SQL dump import has been cancelled.");
                            return ImportSqlDumpResult.CANCELLED;
                        }
                        Logger.Debug("Importing data.");
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
                            Logger.Debug("SQL dump import has been cancelled.");
                            return ImportSqlDumpResult.CANCELLED;
                        }
                        Logger.Debug("SQL dump import has been completed successfully.");
                        return ImportSqlDumpResult.COMPLETED;
                    }
                }
            });
        }

        public Task<SynchronizationResult> SynchronizeNonFiction(IProgress<object> progressHandler, CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                Logger.Debug("Synchronization has started.");
                if (NonFictionBookCount == 0)
                {
                    throw new Exception("Non-fiction table must not be empty.");
                }
                CheckAndCreateNonFictionIndexes(progressHandler, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                {
                    Logger.Debug("Synchronization has been cancelled.");
                    return SynchronizationResult.CANCELLED;
                }
                Logger.Debug("Loading last non-fiction book.");
                NonFictionBook lastModifiedNonFictionBook = localDatabase.GetLastModifiedNonFictionBook();
                Logger.Debug($"Last non-fiction book: Libgen ID = {lastModifiedNonFictionBook.LibgenId}, last modified date/time = {lastModifiedNonFictionBook.LastModifiedDateTime}.");
                string jsonApiUrl = Mirrors[AppSettings.Mirrors.NonFictionSynchronizationMirrorName].NonFictionSynchronizationUrl;
                if (jsonApiUrl == null)
                {
                    throw new Exception("JSON API URL is null.");
                }
                JsonApiClient jsonApiClient = new JsonApiClient(HttpClient, jsonApiUrl, lastModifiedNonFictionBook.LastModifiedDateTime,
                    lastModifiedNonFictionBook.LibgenId);
                List<NonFictionBook> downloadedBooks = new List<NonFictionBook>();
                progressHandler.Report(new JsonApiDownloadProgress(0));
                while (true)
                {
                    List<NonFictionBook> currentBatch;
                    try
                    {
                        Logger.Debug("Downloading next batch from the server.");
                        currentBatch = await jsonApiClient.DownloadNextBatchAsync(cancellationToken);
                    }
                    catch (TaskCanceledException)
                    {
                        Logger.Debug("Synchronization has been cancelled.");
                        return SynchronizationResult.CANCELLED;
                    }
                    if (!currentBatch.Any())
                    {
                        Logger.Debug("Current batch is empty, download is complete.");
                        break;
                    }
                    downloadedBooks.AddRange(currentBatch);
                    Logger.Debug($"{downloadedBooks.Count} books have been downloaded so far.");
                    progressHandler.Report(new JsonApiDownloadProgress(downloadedBooks.Count));
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Logger.Debug("Synchronization has been cancelled.");
                        return SynchronizationResult.CANCELLED;
                    }
                }
                NonFictionImporter importer = new NonFictionImporter(localDatabase, lastModifiedNonFictionBook);
                Logger.Debug("Importing data.");
                importer.Import(downloadedBooks, progressHandler, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                {
                    Logger.Debug("Synchronization has been cancelled.");
                    return SynchronizationResult.CANCELLED;
                }
                Logger.Debug("Synchronization has been completed successfully.");
                return SynchronizationResult.COMPLETED;
            });
        }

        public void SaveSettings()
        {
            SettingsStorage.SaveSettings(AppSettings, Environment.AppSettingsFilePath);
        }

        public string GetDatabaseNormalizedPath(string databaseFullPath)
        {
            if (Environment.IsInPortableMode)
            {
                if (databaseFullPath.ToLower().StartsWith(Environment.AppDataDirectory.ToLower()))
                {
                    return databaseFullPath.Substring(Environment.AppDataDirectory.Length + 1);
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
                return Path.GetFullPath(Path.Combine(Environment.AppDataDirectory, databaseNormalizedPath));
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

        public void CreateNewHttpClient()
        {
            AppSettings.NetworkSettings networkSettings = AppSettings.Network;
            WebProxy webProxy;
            if (networkSettings.UseProxy)
            {
                webProxy = new WebProxy(networkSettings.ProxyAddress, networkSettings.ProxyPort.Value);
                if (!String.IsNullOrEmpty(networkSettings.ProxyUserName))
                {
                    webProxy.Credentials = new NetworkCredential(networkSettings.ProxyUserName, networkSettings.ProxyPassword);
                }
            }
            else
            {
                webProxy = new WebProxy();
            }
            HttpClientHandler httpClientHandler = new HttpClientHandler
            {
                Proxy = webProxy,
                UseProxy = true
            };
            HttpClient = new HttpClient(httpClientHandler);
            HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd(USER_AGENT);
        }

        public void EnableLogging()
        {
            Logger.EnableLogging();
        }

        public void DisableLogging()
        {
            Logger.DisableLogging();
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
                        Logger.Debug("Search has been cancelled.");
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
                Logger.Debug($"Search complete, returning {result.Count} items.");
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
            Logger.Debug("Retrieving the list of non-fiction table indexes.");
            List<string> nonFictionIndexes = localDatabase.GetNonFictionIndexList();
            if (cancellationToken.IsCancellationRequested)
            {
                Logger.Debug("Index check has been cancelled.");
                return;
            }
            Logger.Debug("Checking the index on LastModifiedDateTime column.");
            CheckAndCreateIndex(nonFictionIndexes, SqlScripts.NON_FICTION_INDEX_PREFIX, "LastModifiedDateTime", progressHandler,
                localDatabase.CreateNonFictionLastModifiedDateTimeIndex);
            if (cancellationToken.IsCancellationRequested)
            {
                Logger.Debug("Index check has been cancelled.");
                return;
            }
            Logger.Debug("Checking the index on LibgenId column.");
            CheckAndCreateIndex(nonFictionIndexes, SqlScripts.NON_FICTION_INDEX_PREFIX, "LibgenId", progressHandler,
                localDatabase.CreateNonFictionLibgenIdIndex);
        }

        private void CheckAndCreateFictionIndexes(IProgress<object> progressHandler, CancellationToken cancellationToken)
        {
            Logger.Debug("Retrieving the list of fiction table indexes.");
            List<string> fictionIndexes = localDatabase.GetFictionIndexList();
            if (cancellationToken.IsCancellationRequested)
            {
                Logger.Debug("Index check has been cancelled.");
                return;
            }
            Logger.Debug("Checking the index on LastModifiedDateTime column.");
            CheckAndCreateIndex(fictionIndexes, SqlScripts.FICTION_INDEX_PREFIX, "LastModifiedDateTime", progressHandler,
                localDatabase.CreateFictionLastModifiedDateTimeIndex);
            if (cancellationToken.IsCancellationRequested)
            {
                Logger.Debug("Index check has been cancelled.");
                return;
            }
            Logger.Debug("Checking the index on LibgenId column.");
            CheckAndCreateIndex(fictionIndexes, SqlScripts.FICTION_INDEX_PREFIX, "LibgenId", progressHandler, localDatabase.CreateFictionLibgenIdIndex);
        }

        private void CheckAndCreateSciMagIndexes(IProgress<object> progressHandler, CancellationToken cancellationToken)
        {
            Logger.Debug("Retrieving the list of scimag table indexes.");
            List<string> sciMagIndexes = localDatabase.GetSciMagIndexList();
            if (cancellationToken.IsCancellationRequested)
            {
                Logger.Debug("Index check has been cancelled.");
                return;
            }
            Logger.Debug("Checking the index on AddedDateTime column.");
            CheckAndCreateIndex(sciMagIndexes, SqlScripts.SCIMAG_INDEX_PREFIX, "AddedDateTime", progressHandler, localDatabase.CreateSciMagAddedDateTimeIndex);
        }

        private void CheckAndCreateIndex(List<string> existingIndexes, string prefix, string fieldName, IProgress<object> progressHandler,
            Action createIndexAction)
        {
            if (!existingIndexes.Contains(prefix + fieldName))
            {
                Logger.Debug($"Index on {fieldName} doesn't exist, creating it.");
                progressHandler.Report(new ImportCreateIndexProgress(fieldName));
                createIndexAction();
                Logger.Debug("Index has been created.");
            }
        }

        private void UpdateNonFictionBookCount()
        {
            Logger.Debug("Updating non-fiction book count.");
            NonFictionBookCount = localDatabase.CountNonFictionBooks();
            Logger.Debug($"Non-fiction book count = {NonFictionBookCount}.");
        }

        private void UpdateFictionBookCount()
        {
            Logger.Debug("Updating fiction book count.");
            FictionBookCount = localDatabase.CountFictionBooks();
            Logger.Debug($"Fiction book count = {FictionBookCount}.");
        }

        private void UpdateSciMagArticleCount()
        {
            Logger.Debug("Updating article count.");
            SciMagArticleCount = localDatabase.CountSciMagArticles();
            Logger.Debug($"Article count = {SciMagArticleCount}.");
        }

        private void ValidateAndCorrectSelectedMirrors()
        {
            AppSettings.MirrorSettings mirrorSettings = AppSettings.Mirrors;
            mirrorSettings.NonFictionBooksMirrorName =
                ValidateAndCorrectSelectedMirror(mirrorSettings.NonFictionBooksMirrorName, mirror => mirror.NonFictionDownloadUrl);
            mirrorSettings.NonFictionCoversMirrorName =
                ValidateAndCorrectSelectedMirror(mirrorSettings.NonFictionCoversMirrorName, mirror => mirror.NonFictionCoverUrl);
            mirrorSettings.NonFictionSynchronizationMirrorName =
                ValidateAndCorrectSelectedMirror(mirrorSettings.NonFictionSynchronizationMirrorName, mirror => mirror.NonFictionSynchronizationUrl);
            mirrorSettings.FictionBooksMirrorName =
                ValidateAndCorrectSelectedMirror(mirrorSettings.FictionBooksMirrorName, mirror => mirror.FictionDownloadUrl);
            mirrorSettings.FictionCoversMirrorName =
                ValidateAndCorrectSelectedMirror(mirrorSettings.FictionCoversMirrorName, mirror => mirror.FictionCoverUrl);
            mirrorSettings.ArticlesMirrorMirrorName =
                ValidateAndCorrectSelectedMirror(mirrorSettings.ArticlesMirrorMirrorName, mirror => mirror.SciMagDownloadUrl);
        }

        private string ValidateAndCorrectSelectedMirror(string selectedMirrorName, Func<Mirrors.MirrorConfiguration, string> mirrorProperty)
        {
            if (selectedMirrorName != null && Mirrors.ContainsKey(selectedMirrorName) &&
                !String.IsNullOrWhiteSpace(mirrorProperty(Mirrors[selectedMirrorName])))
            {
                return selectedMirrorName;
            }
            return null;
        }
    }
}
