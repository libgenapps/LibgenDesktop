using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using LibgenDesktop.Common;
using LibgenDesktop.Models.Database;
using LibgenDesktop.Models.Download;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.Export;
using LibgenDesktop.Models.Import;
using LibgenDesktop.Models.JsonApi;
using LibgenDesktop.Models.Localization;
using LibgenDesktop.Models.ProgressArgs;
using LibgenDesktop.Models.Settings;
using LibgenDesktop.Models.SqlDump;
using LibgenDesktop.Models.Update;
using LibgenDesktop.Models.Utils;
using static LibgenDesktop.Common.Constants;
using Environment = LibgenDesktop.Common.Environment;

namespace LibgenDesktop.Models
{
    internal class MainModel : IDisposable
    {
        internal enum DatabaseStatus
        {
            OPENED = 1,
            NOT_FOUND,
            NOT_SET,
            CORRUPTED,
            SERVER_DATABASE
        }

        internal enum ImportSqlDumpResult
        {
            COMPLETED = 1,
            CANCELLED,
            DATA_NOT_FOUND,
            LOW_DISK_SPACE
        }

        internal enum SynchronizationResult
        {
            COMPLETED = 1,
            CANCELLED,
            LOW_DISK_SPACE
        }

        internal enum DownloadFileResult
        {
            COMPLETED = 1,
            INCOMPLETE,
            CANCELLED
        }

        private readonly Updater updater;
        private LocalDatabase localDatabase;
        private bool disposed;

        public MainModel()
        {
            AppSettings = SettingsStorage.LoadSettings(Environment.AppSettingsFilePath);
            if (AppSettings.Advanced.LoggingEnabled)
            {
                EnableLogging();
            }
            ValidateAndCorrectDirectoryPaths();
            Mirrors = MirrorStorage.LoadMirrors(Path.Combine(Environment.MirrorsDirectoryPath, MIRRORS_FILE_NAME));
            ValidateAndCorrectSelectedMirrors();
            Localization = new LocalizationStorage(Environment.LanguagesDirectoryPath, AppSettings.General.Language);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 |
                SecurityProtocolType.Tls12;
            CreateNewHttpClient();
            OpenDatabase(AppSettings.DatabaseFileName);
            LastApplicationUpdateCheckDateTime = AppSettings.LastUpdate.LastCheckedAt;
            LastApplicationUpdateCheckResult = null;
            updater = new Updater();
            updater.UpdateCheck += ApplicationUpdateCheck;
            ConfigureUpdater();
            Downloader = new Downloader();
            ConfigureDownloader();
            CoverCache = new CoverCache(COVER_CACHE_CAPACITY, HttpClient, AppSettings.Network.OfflineMode);
            disposed = false;
        }

        public AppSettings AppSettings { get; }
        public DatabaseStatus LocalDatabaseStatus { get; private set; }
        public DatabaseMetadata DatabaseMetadata { get; private set; }
        public Mirrors Mirrors { get; }
        public LocalizationStorage Localization { get; }
        public HttpClient HttpClient { get; private set; }
        public Downloader Downloader { get; }
        public CoverCache CoverCache { get; }
        public int NonFictionBookCount { get; private set; }
        public int FictionBookCount { get; private set; }
        public int SciMagArticleCount { get; private set; }
        public DateTime? LastApplicationUpdateCheckDateTime { get; set; }
        public Updater.UpdateCheckResult LastApplicationUpdateCheckResult { get; set; }

        public event EventHandler ApplicationUpdateCheckCompleted;
        public event EventHandler AppSettingsChanged;
        public event EventHandler BookmarksChanged;

        public Task<List<NonFictionBook>> SearchNonFictionAsync(string searchQuery, IProgress<SearchProgress> progressHandler,
            CancellationToken cancellationToken)
        {
            return SearchItemsAsync(localDatabase.SearchNonFictionBooks, searchQuery, progressHandler, cancellationToken);
        }

        public Task<ExportResult> ExportNonFictionToXlsxAsync(string filePathTemplate, string fileExtension, string searchQuery,
            int? searchResultLimit, IProgress<ExportProgress> progressHandler, CancellationToken cancellationToken)
        {
            return ExportToXlsxAsync(localDatabase.SearchNonFictionBooks, filePathTemplate, fileExtension,
                xlsxExporter => xlsxExporter.ExportNonFiction, searchQuery, searchResultLimit, progressHandler, cancellationToken);
        }

        public Task<ExportResult> ExportNonFictionToCsvAsync(string filePathTemplate, string fileExtension, char separator, string searchQuery,
            int? searchResultLimit, IProgress<ExportProgress> progressHandler, CancellationToken cancellationToken)
        {
            return ExportToCsvAsync(localDatabase.SearchNonFictionBooks, filePathTemplate, fileExtension, separator,
                csvExporter => csvExporter.ExportNonFiction, searchQuery, searchResultLimit, progressHandler, cancellationToken);
        }

        public Task<NonFictionBook> LoadNonFictionBookAsync(int bookId)
        {
            Logger.Debug($"Loading non-fiction book with ID = {bookId}.");
            return LoadItemAsync(localDatabase.GetNonFictionBookById, bookId);
        }

        public Task<List<FictionBook>> SearchFictionAsync(string searchQuery, IProgress<SearchProgress> progressHandler,
            CancellationToken cancellationToken)
        {
            return SearchItemsAsync(localDatabase.SearchFictionBooks, searchQuery, progressHandler, cancellationToken);
        }

        public Task<ExportResult> ExportFictionToXlsxAsync(string filePathTemplate, string fileExtension, string searchQuery,
            int? searchResultLimit, IProgress<ExportProgress> progressHandler, CancellationToken cancellationToken)
        {
            return ExportToXlsxAsync(localDatabase.SearchFictionBooks, filePathTemplate, fileExtension, xlsxExporter => xlsxExporter.ExportFiction,
                searchQuery, searchResultLimit, progressHandler, cancellationToken);
        }

        public Task<ExportResult> ExportFictionToCsvAsync(string filePathTemplate, string fileExtension, char separator, string searchQuery,
            int? searchResultLimit, IProgress<ExportProgress> progressHandler, CancellationToken cancellationToken)
        {
            return ExportToCsvAsync(localDatabase.SearchFictionBooks, filePathTemplate, fileExtension, separator, csvExporter => csvExporter.ExportFiction,
                searchQuery, searchResultLimit, progressHandler, cancellationToken);
        }

        public Task<FictionBook> LoadFictionBookAsync(int bookId)
        {
            Logger.Debug($"Loading fiction book with ID = {bookId}.");
            return LoadItemAsync(localDatabase.GetFictionBookById, bookId);
        }

        public Task<List<SciMagArticle>> SearchSciMagAsync(string searchQuery, IProgress<SearchProgress> progressHandler,
            CancellationToken cancellationToken)
        {
            return SearchItemsAsync(localDatabase.SearchSciMagArticles, searchQuery, progressHandler, cancellationToken);
        }

        public Task<ExportResult> ExportSciMagToXlsxAsync(string filePathTemplate, string fileExtension, string searchQuery,
            int? searchResultLimit, IProgress<ExportProgress> progressHandler, CancellationToken cancellationToken)
        {
            return ExportToXlsxAsync(localDatabase.SearchSciMagArticles, filePathTemplate, fileExtension, xlsxExporter => xlsxExporter.ExportSciMag,
                searchQuery, searchResultLimit, progressHandler, cancellationToken);
        }

        public Task<ExportResult> ExportSciMagToCsvAsync(string filePathTemplate, string fileExtension, char separator, string searchQuery,
            int? searchResultLimit, IProgress<ExportProgress> progressHandler, CancellationToken cancellationToken)
        {
            return ExportToCsvAsync(localDatabase.SearchSciMagArticles, filePathTemplate, fileExtension, separator, csvExporter => csvExporter.ExportSciMag,
                searchQuery, searchResultLimit, progressHandler, cancellationToken);
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
                long? freeSpaceAtStart = GetFreeSpaceOnDatabaseDiskDrive();
                progressHandler.Report(new ImportDiskSpaceProgress(freeSpaceAtStart));
                if (freeSpaceAtStart.HasValue && freeSpaceAtStart.Value < LOW_DISK_SPACE_THRESHOLD_BYTES)
                {
                    Logger.Debug($"Insufficient disk space: {freeSpaceAtStart.Value} bytes.");
                    return ImportSqlDumpResult.LOW_DISK_SPACE;
                }
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
                        BitArray existingLibgenIds = null;
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
                                    progressHandler.Report(new ImportLoadLibgenIdsProgress());
                                    existingLibgenIds = localDatabase.GetNonFictionLibgenIdsBitArray();
                                }
                                importer = new NonFictionImporter(localDatabase, existingLibgenIds);
                                break;
                            case TableType.FICTION:
                                if (FictionBookCount != 0)
                                {
                                    CheckAndCreateFictionIndexes(progressHandler, cancellationToken);
                                    if (cancellationToken.IsCancellationRequested)
                                    {
                                        return ImportSqlDumpResult.CANCELLED;
                                    }
                                    progressHandler.Report(new ImportLoadLibgenIdsProgress());
                                    existingLibgenIds = localDatabase.GetFictionLibgenIdsBitArray();
                                }
                                importer = new FictionImporter(localDatabase, existingLibgenIds);
                                break;
                            case TableType.SCI_MAG:
                                if (SciMagArticleCount != 0)
                                {
                                    CheckAndCreateSciMagIndexes(progressHandler, cancellationToken);
                                    if (cancellationToken.IsCancellationRequested)
                                    {
                                        return ImportSqlDumpResult.CANCELLED;
                                    }
                                    progressHandler.Report(new ImportLoadLibgenIdsProgress());
                                    existingLibgenIds = localDatabase.GetSciMagLibgenIdsBitArray();
                                }
                                importer = new SciMagImporter(localDatabase, existingLibgenIds);
                                break;
                            default:
                                throw new Exception($"Unknown table type: {tableType}.");
                        }
                        if (cancellationToken.IsCancellationRequested)
                        {
                            Logger.Debug("SQL dump import has been cancelled.");
                            return ImportSqlDumpResult.CANCELLED;
                        }
                        Importer.ImportProgressReporter importProgressReporter = (int objectsAdded, int objectsUpdated, long? freeSpace) =>
                        {
                            progressHandler.Report(new ImportObjectsProgress(objectsAdded, objectsUpdated));
                            progressHandler.Report(new ImportDiskSpaceProgress(freeSpace));
                        };
                        Logger.Debug("Importing data.");
                        Importer.ImportResult importResult = importer.Import(sqlDumpReader, importProgressReporter, IMPORT_PROGRESS_UPDATE_INTERVAL,
                            cancellationToken, parsedTableDefinition, localDatabase.DatabaseFullPath);
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
                        switch (tableType)
                        {
                            case TableType.NON_FICTION:
                                DatabaseMetadata.NonFictionFirstImportComplete = true;
                                break;
                            case TableType.FICTION:
                                DatabaseMetadata.FictionFirstImportComplete = true;
                                break;
                            case TableType.SCI_MAG:
                                DatabaseMetadata.SciMagFirstImportComplete = true;
                                break;
                        }
                        localDatabase.UpdateMetadata(DatabaseMetadata);
                        if (cancellationToken.IsCancellationRequested)
                        {
                            Logger.Debug("SQL dump import has been cancelled by the user.");
                            return ImportSqlDumpResult.CANCELLED;
                        }
                        if (importResult.ErrorLowDiskSpace)
                        {
                            Logger.Debug("SQL dump import has been cancelled due to insufficient disk space.");
                            return ImportSqlDumpResult.LOW_DISK_SPACE;
                        }
                        Logger.Debug("SQL dump import has been completed successfully.");
                        return ImportSqlDumpResult.COMPLETED;
                    }
                }
            });
        }

        public Task<SynchronizationResult> SynchronizeNonFictionAsync(IProgress<object> progressHandler, CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                Logger.Debug("Synchronization has started.");
                if (NonFictionBookCount == 0)
                {
                    throw new Exception("Non-fiction table must not be empty.");
                }
                long? freeSpaceAtStart = GetFreeSpaceOnDatabaseDiskDrive();
                progressHandler.Report(new SynchronizationDiskSpaceProgress(freeSpaceAtStart));
                if (freeSpaceAtStart.HasValue && freeSpaceAtStart.Value < LOW_DISK_SPACE_THRESHOLD_BYTES)
                {
                    Logger.Debug($"Insufficient disk space: {freeSpaceAtStart.Value} bytes.");
                    return SynchronizationResult.LOW_DISK_SPACE;
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
                progressHandler.Report(new ImportLoadLibgenIdsProgress());
                BitArray existingLibgenIds = localDatabase.GetNonFictionLibgenIdsBitArray();
                NonFictionImporter importer = new NonFictionImporter(localDatabase, existingLibgenIds, lastModifiedNonFictionBook);
                progressHandler.Report(new SynchronizationObjectsProgress(0, 0, 0));
                int downloadedBookCount = 0;
                int totalAddedBookCount = 0;
                int totalUpdatedBookCount = 0;
                Importer.ImportProgressReporter importProgressReporter = (int objectsAdded, int objectsUpdated, long? freeSpace) =>
                {
                    progressHandler.Report(new SynchronizationObjectsProgress(downloadedBookCount, totalAddedBookCount + objectsAdded,
                        totalUpdatedBookCount + objectsUpdated));
                    progressHandler.Report(new SynchronizationDiskSpaceProgress(freeSpace));
                };
                Importer.ImportResult importResult = null;
                bool isCancelled = false;
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
                        isCancelled = true;
                        break;
                    }
                    if (!currentBatch.Any())
                    {
                        Logger.Debug("Current batch is empty, download is complete.");
                        break;
                    }
                    downloadedBookCount += currentBatch.Count;
                    Logger.Debug($"Batch download is complete, {downloadedBookCount} books have been downloaded so far.");
                    progressHandler.Report(new SynchronizationObjectsProgress(downloadedBookCount, totalAddedBookCount, totalUpdatedBookCount));
                    if (cancellationToken.IsCancellationRequested)
                    {
                        isCancelled = true;
                        break;
                    }
                    Logger.Debug("Importing downloaded batch.");
                    importResult = importer.Import(currentBatch, importProgressReporter, SYNCHRONIZATION_PROGRESS_UPDATE_INTERVAL,
                        cancellationToken, localDatabase.DatabaseFullPath);
                    totalAddedBookCount += importResult.AddedObjectCount;
                    totalUpdatedBookCount += importResult.UpdatedObjectCount;
                    Logger.Debug($"Batch has been imported, total added book count = {totalAddedBookCount}, total updated book count = {totalUpdatedBookCount}.");
                    if (!importResult.IsSuccessful || cancellationToken.IsCancellationRequested)
                    {
                        isCancelled = cancellationToken.IsCancellationRequested;
                        break;
                    }
                }
                if (isCancelled)
                {
                    Logger.Debug("Synchronization has been cancelled.");
                    return SynchronizationResult.CANCELLED;
                }
                if (importResult != null && importResult.ErrorLowDiskSpace)
                {
                    Logger.Debug("Synchronization has been cancelled due to insufficient disk space.");
                    return SynchronizationResult.LOW_DISK_SPACE;
                }
                Logger.Debug("Synchronization has been completed successfully.");
                return SynchronizationResult.COMPLETED;
            });
        }

        public Task<DownloadFileResult> DownloadFileAsync(string fileUrl, string destinationPath, IProgress<object> progressHandler,
            CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                Logger.Debug($"Requesting {fileUrl}");
                HttpResponseMessage response;
                try
                {
                    response = await HttpClient.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    Logger.Debug("File download has been cancelled.");
                    return DownloadFileResult.CANCELLED;
                }
                Logger.Debug($"Response status code: {(int)response.StatusCode} {response.StatusCode}.");
                Logger.Debug("Response headers:", response.Headers.ToString().TrimEnd(), response.Content.Headers.ToString().TrimEnd());
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception($"Server returned {(int)response.StatusCode} {response.StatusCode}.");
                }
                long? contentLength = response.Content.Headers.ContentLength;
                if (!contentLength.HasValue)
                {
                    throw new Exception($"Server did not return Content-Length value.");
                }
                int fileSize = (int)contentLength.Value;
                Logger.Debug($"File size is {fileSize} bytes.");
                Stream downloadStream = await response.Content.ReadAsStreamAsync();
                byte[] buffer = new byte[4096];
                int downloadedBytes = 0;
                using (FileStream destinationFileStream = new FileStream(destinationPath, FileMode.Create))
                {
                    while (true)
                    {
                        int bytesRead;
                        try
                        {
                            bytesRead = downloadStream.Read(buffer, 0, buffer.Length);
                        }
                        catch
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                Logger.Debug("File download has been cancelled.");
                                return DownloadFileResult.CANCELLED;
                            }
                            throw;
                        }
                        if (bytesRead == 0)
                        {
                            bool isCompleted = downloadedBytes == fileSize;
                            Logger.Debug($"File download is {(isCompleted ? "complete" : "incomplete")}.");
                            return isCompleted ? DownloadFileResult.COMPLETED : DownloadFileResult.INCOMPLETE;
                        }
                        destinationFileStream.Write(buffer, 0, bytesRead);
                        downloadedBytes += bytesRead;
                        progressHandler.Report(new DownloadFileProgress(downloadedBytes, fileSize));
                    }
                }
            });
        }

        public Task ScanNonFictionAsync(string scanDirectory, IProgress<object> progressHandler)
        {
            return ScanAsync(scanDirectory, progressHandler, NonFictionBookCount, localDatabase.GetNonFictionIndexList,
                SqlScripts.NON_FICTION_INDEX_PREFIX, localDatabase.CreateNonFictionMd5HashIndex, localDatabase.GetNonFictionBookByMd5Hash);
        }

        public Task ScanFictionAsync(string scanDirectory, IProgress<object> progressHandler)
        {
            return ScanAsync(scanDirectory, progressHandler, FictionBookCount, localDatabase.GetFictionIndexList,
                SqlScripts.FICTION_INDEX_PREFIX, localDatabase.CreateFictionMd5HashIndex, localDatabase.GetFictionBookByMd5Hash);
        }

        public Task ScanSciMagAsync(string scanDirectory, IProgress<object> progressHandler)
        {
            return ScanAsync(scanDirectory, progressHandler, SciMagArticleCount, localDatabase.GetSciMagIndexList,
                SqlScripts.SCIMAG_INDEX_PREFIX, localDatabase.CreateSciMagMd5HashIndex, localDatabase.GetSciMagArticleByMd5Hash);
        }

        public Task<LibraryFile> LoadFileAsync(int fileId)
        {
            Logger.Debug($"Loading file with ID = {fileId}.");
            return LoadItemAsync(localDatabase.GetFileById, fileId);
        }

        public Task AddFiles(List<LibraryFile> files)
        {
            return Task.Run(() =>
            {
                Logger.Debug($"Adding {files.Count} files to the database.");
                localDatabase.AddFiles(files);
                Logger.Debug($"{files.Count} files have been added to the database.");
            });
        }

        public Task<bool> CheckIfDatabaseStatsIndexesCreated()
        {
            return Task.Run(() =>
            {
                if (NonFictionBookCount > 0)
                {
                    Logger.Debug("Retrieving the list of non-fiction table indexes.");
                    List<string> nonFictionIndexes = localDatabase.GetNonFictionIndexList();
                    Logger.Debug("Checking the index on LastModifiedDateTime column.");
                    if (!CheckIfIndexExists(nonFictionIndexes, SqlScripts.NON_FICTION_INDEX_PREFIX, "LastModifiedDateTime"))
                    {
                        return false;
                    }
                }
                if (FictionBookCount > 0)
                {
                    Logger.Debug("Retrieving the list of fiction table indexes.");
                    List<string> fictionIndexes = localDatabase.GetFictionIndexList();
                    Logger.Debug("Checking the index on LastModifiedDateTime column.");
                    if (!CheckIfIndexExists(fictionIndexes, SqlScripts.FICTION_INDEX_PREFIX, "LastModifiedDateTime"))
                    {
                        return false;
                    }
                }
                if (SciMagArticleCount > 0)
                {
                    Logger.Debug("Retrieving the list of scimag table indexes.");
                    List<string> sciMagIndexes = localDatabase.GetSciMagIndexList();
                    Logger.Debug("Checking the index on AddedDateTime column.");
                    if (!CheckIfIndexExists(sciMagIndexes, SqlScripts.SCIMAG_INDEX_PREFIX, "AddedDateTime"))
                    {
                        return false;
                    }
                }
                return true;
            });
        }

        public Task<DatabaseStats> GetDatabaseStatsAsync()
        {
            return Task.Run(() =>
            {
                if (NonFictionBookCount > 0)
                {
                    Logger.Debug("Retrieving the list of non-fiction table indexes.");
                    List<string> nonFictionIndexes = localDatabase.GetNonFictionIndexList();
                    Logger.Debug("Checking the index on LastModifiedDateTime column.");
                    CheckAndCreateIndex(nonFictionIndexes, SqlScripts.NON_FICTION_INDEX_PREFIX, "LastModifiedDateTime", null,
                        localDatabase.CreateNonFictionLastModifiedDateTimeIndex);
                }
                if (FictionBookCount > 0)
                {
                    Logger.Debug("Retrieving the list of fiction table indexes.");
                    List<string> fictionIndexes = localDatabase.GetFictionIndexList();
                    Logger.Debug("Checking the index on LastModifiedDateTime column.");
                    CheckAndCreateIndex(fictionIndexes, SqlScripts.FICTION_INDEX_PREFIX, "LastModifiedDateTime", null,
                        localDatabase.CreateFictionLastModifiedDateTimeIndex);
                }
                if (SciMagArticleCount > 0)
                {
                    Logger.Debug("Retrieving the list of scimag table indexes.");
                    List<string> sciMagIndexes = localDatabase.GetSciMagIndexList();
                    Logger.Debug("Checking the index on AddedDateTime column.");
                    CheckAndCreateIndex(sciMagIndexes, SqlScripts.SCIMAG_INDEX_PREFIX, "AddedDateTime", null, localDatabase.CreateSciMagAddedDateTimeIndex);
                }
                DatabaseStats result = new DatabaseStats
                {
                    NonFictionBookCount = NonFictionBookCount,
                    FictionBookCount = FictionBookCount,
                    SciMagArticleCount = SciMagArticleCount
                };
                if (result.NonFictionBookCount > 0)
                {
                    result.NonFictionLastUpdate = localDatabase.GetLastModifiedNonFictionBook()?.LastModifiedDateTime;
                }
                if (result.FictionBookCount > 0)
                {
                    result.FictionLastUpdate = localDatabase.GetLastModifiedFictionBook()?.LastModifiedDateTime;
                }
                if (result.SciMagArticleCount > 0)
                {
                    result.SciMagLastUpdate = localDatabase.GetLastAddedSciMagArticle()?.AddedDateTime;
                }
                return result;
            });
        }

        public async Task<Updater.UpdateCheckResult> CheckForApplicationUpdateAsync()
        {
            Updater.UpdateCheckResult result = await updater.CheckForUpdateAsync(false);
            if (result != null && result.NewReleaseName != AppSettings.LastUpdate.IgnoreReleaseName)
            {
                LastApplicationUpdateCheckResult = result;
                ApplicationUpdateCheckCompleted?.Invoke(this, EventArgs.Empty);
            }
            LastApplicationUpdateCheckDateTime = DateTime.Now;
            if (result == null)
            {
                AppSettings.LastUpdate.LastCheckedAt = LastApplicationUpdateCheckDateTime;
                SaveSettings();
            }
            ConfigureUpdater();
            return result;
        }

        public bool HasBookmark(LibgenObjectType libgenObjectType, string searchQuery)
        {
            if (String.IsNullOrWhiteSpace(searchQuery))
            {
                return false;
            }
            searchQuery = searchQuery.Trim();
            return AppSettings.Bookmarks.Bookmarks.Any(bookmark => bookmark.Type == libgenObjectType && bookmark.Query == searchQuery);
        }

        public void AddBookmark(LibgenObjectType libgenObjectType, string name, string searchQuery)
        {
            AppSettings.Bookmarks.Bookmarks.Add(new AppSettings.BookmarkSettings.Bookmark
            {
                Name = name,
                Query = searchQuery,
                Type = libgenObjectType
            });
            SaveSettings();
            BookmarksChanged?.Invoke(this, EventArgs.Empty);
        }

        public void DeleteBookmark(LibgenObjectType libgenObjectType, string searchQuery)
        {
            if (String.IsNullOrWhiteSpace(searchQuery))
            {
                return;
            }
            searchQuery = searchQuery.Trim();
            AppSettings.BookmarkSettings.Bookmark bookmark =
                AppSettings.Bookmarks.Bookmarks.FirstOrDefault(bookmarkItem => bookmarkItem.Type == libgenObjectType && bookmarkItem.Query == searchQuery);
            if (bookmark != null)
            {
                AppSettings.Bookmarks.Bookmarks.Remove(bookmark);
                SaveSettings();
                BookmarksChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void SaveSettings()
        {
            SettingsStorage.SaveSettings(AppSettings, Environment.AppSettingsFilePath);
            AppSettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ReconfigureSettingsDependencies()
        {
            Localization.SwitchLanguage(AppSettings.General.Language);
            CreateNewHttpClient();
            ConfigureUpdater();
            ConfigureDownloader();
            ConfigureCoverCache();
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
                        string databaseFullPath = GetDatabaseFullPath(databaseFilePath);
                        localDatabase = LocalDatabase.OpenDatabase(databaseFullPath);
                        if (!localDatabase.CheckIfMetadataExists())
                        {
                            LocalDatabaseStatus = DatabaseStatus.CORRUPTED;
                            return false;
                        }
                        DatabaseMetadata = localDatabase.GetMetadata();
                        if (!String.IsNullOrEmpty(DatabaseMetadata.AppName) &&
                            DatabaseMetadata.AppName.CompareOrdinalIgnoreCase(LIBGEN_SERVER_DATABASE_METADATA_APP_NAME))
                        {
                            LocalDatabaseStatus = DatabaseStatus.SERVER_DATABASE;
                            return false;
                        }
                        if (String.IsNullOrEmpty(DatabaseMetadata.Version))
                        {
                            LocalDatabaseStatus = DatabaseStatus.CORRUPTED;
                            return false;
                        }
                        if (!Migration.Migrate(localDatabase, DatabaseMetadata))
                        {
                            LocalDatabaseStatus = DatabaseStatus.CORRUPTED;
                            return false;
                        }
                        UpdateNonFictionBookCount();
                        UpdateFictionBookCount();
                        UpdateSciMagArticleCount();
                    }
                    catch (Exception exception)
                    {
                        Logger.Exception(exception);
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
                string databaseFullPath = GetDatabaseFullPath(databaseFilePath);
                localDatabase = LocalDatabase.CreateDatabase(databaseFullPath);
                localDatabase.CreateMetadataTable();
                localDatabase.CreateFilesTable();
                localDatabase.CreateNonFictionTables();
                localDatabase.CreateFictionTables();
                localDatabase.CreateSciMagTables();
                DatabaseMetadata = new DatabaseMetadata
                {
                    Version = CURRENT_DATABASE_VERSION,
                    NonFictionFirstImportComplete = false,
                    FictionFirstImportComplete = false,
                    SciMagFirstImportComplete = false
                };
                localDatabase.AddMetadata(DatabaseMetadata);
                NonFictionBookCount = 0;
                FictionBookCount = 0;
                SciMagArticleCount = 0;
                return true;
            }
            catch (Exception exception)
            {
                Logger.Exception(exception);
                LocalDatabaseStatus = DatabaseStatus.CORRUPTED;
                return false;
            }
        }

        public void CreateNewHttpClient()
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler
            {
                Proxy = NetworkUtils.CreateProxy(AppSettings.Network),
                UseProxy = true,
                AllowAutoRedirect = true,
                UseCookies = false
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

        public void ConfigureUpdater()
        {
            DateTime? nextUpdateCheck;
            if (AppSettings.General.UpdateCheck == AppSettings.GeneralSettings.UpdateCheckInterval.NEVER || AppSettings.Network.OfflineMode)
            {
                nextUpdateCheck = null;
            }
            else
            {
                if (!LastApplicationUpdateCheckDateTime.HasValue)
                {
                    nextUpdateCheck = DateTime.Now;
                }
                else
                {
                    switch (AppSettings.General.UpdateCheck)
                    {
                        case AppSettings.GeneralSettings.UpdateCheckInterval.DAILY:
                            nextUpdateCheck = LastApplicationUpdateCheckDateTime.Value.AddDays(1);
                            break;
                        case AppSettings.GeneralSettings.UpdateCheckInterval.WEEKLY:
                            nextUpdateCheck = LastApplicationUpdateCheckDateTime.Value.AddDays(7);
                            break;
                        case AppSettings.GeneralSettings.UpdateCheckInterval.MONTHLY:
                            nextUpdateCheck = LastApplicationUpdateCheckDateTime.Value.AddMonths(1);
                            break;
                        default:
                            throw new Exception($"Unexpected update check interval: {AppSettings.General.UpdateCheck}.");
                    }
                }
            }
            updater.Configure(HttpClient, nextUpdateCheck, AppSettings.LastUpdate.IgnoreReleaseName);
        }

        public void ConfigureDownloader()
        {
            Downloader.Configure(Localization.CurrentLanguage, AppSettings.Network, AppSettings.Download);
        }

        public void ConfigureCoverCache()
        {
            CoverCache.Configure(HttpClient, AppSettings.Network.OfflineMode);
        }

        public Task<string> RunCustomSqlQuery(string sqlQuery)
        {
            return Task.Run(() =>
            {
                return localDatabase.RunCustomSqlQuery(sqlQuery);
            });
        }

        public void Dispose()
        {
            if (!disposed)
            {
                Downloader?.Dispose();
                disposed = true;
            }
        }

        private Task<List<T>> SearchItemsAsync<T>(Func<string, int?, IEnumerable<T>> searchFunction, string searchQuery,
            IProgress<SearchProgress> progressHandler, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                int? resultLimit = null;
                if (AppSettings.Search.LimitResults)
                {
                    resultLimit = AppSettings.Search.MaximumResultCount;
                }
                Logger.Debug($@"Search query = ""{searchQuery}"", result limit = {resultLimit?.ToString() ?? "none"}, object type = {typeof(T).Name}.");
                List<T> result = new List<T>();
                DateTime lastReportDateTime = DateTime.Now;
                foreach (T item in searchFunction(searchQuery, resultLimit))
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Logger.Debug("Search has been cancelled.");
                        return result;
                    }
                    result.Add(item);
                    DateTime now = DateTime.Now;
                    if ((now - lastReportDateTime).TotalSeconds > SEARCH_PROGRESS_REPORT_INTERVAL)
                    {
                        progressHandler.Report(new SearchProgress(result.Count));
                        lastReportDateTime = now;
                    }
                }
                progressHandler.Report(new SearchProgress(result.Count));
                Logger.Debug($"Search complete, returning {result.Count} items.");
                return result;
            });
        }

        private Task<ExportResult> ExportToXlsxAsync<T>(Func<string, int?, IEnumerable<T>> searchFunction, string filePathTemplate, string fileExtension,
            Func<XlsxExporter, Func<IEnumerable<T>, IProgress<ExportProgress>, CancellationToken, ExportResult>> xlsxExporterFunction,
            string searchQuery, int? searchResultLimit, IProgress<ExportProgress> progressHandler, CancellationToken cancellationToken)
        {
            bool splitIntoMultipleFiles = AppSettings.Export.SplitIntoMultipleFiles;
            int? rowsPerFile;
            if (splitIntoMultipleFiles)
            {
                rowsPerFile = AppSettings.Export.MaximumRowsPerFile;
            }
            else
            {
                rowsPerFile = MAX_EXPORT_ROWS_PER_FILE;
            }
            XlsxExporter xlsxExporter = new XlsxExporter(filePathTemplate, fileExtension, rowsPerFile, splitIntoMultipleFiles, Localization.CurrentLanguage);
            Func<IEnumerable<T>, IProgress<ExportProgress>, CancellationToken, ExportResult> exportFunction = xlsxExporterFunction(xlsxExporter);
            return ExportAsync(searchFunction, exportFunction, searchQuery, searchResultLimit, progressHandler, cancellationToken);
        }

        private Task<ExportResult> ExportToCsvAsync<T>(Func<string, int?, IEnumerable<T>> searchFunction, string filePathTemplate, string fileExtension,
            char separator, Func<CsvExporter, Func<IEnumerable<T>, IProgress<ExportProgress>, CancellationToken, ExportResult>> csvExporterFunction,
            string searchQuery, int? searchResultLimit, IProgress<ExportProgress> progressHandler, CancellationToken cancellationToken)
        {
            bool splitIntoMultipleFiles = AppSettings.Export.SplitIntoMultipleFiles;
            int? rowsPerFile;
            if (splitIntoMultipleFiles)
            {
                rowsPerFile = AppSettings.Export.MaximumRowsPerFile;
            }
            else
            {
                rowsPerFile = null;
            }
            CsvExporter csvExporter = new CsvExporter(filePathTemplate, fileExtension, rowsPerFile, splitIntoMultipleFiles, separator,
                Localization.CurrentLanguage);
            Func<IEnumerable<T>, IProgress<ExportProgress>, CancellationToken, ExportResult> exportFunction = csvExporterFunction(csvExporter);
            return ExportAsync(searchFunction, exportFunction, searchQuery, searchResultLimit, progressHandler, cancellationToken);
        }

        private Task<ExportResult> ExportAsync<T>(Func<string, int?, IEnumerable<T>> searchFunction,
            Func<IEnumerable<T>, IProgress<ExportProgress>, CancellationToken, ExportResult> exportFunction,
            string searchQuery, int? searchResultLimit, IProgress<ExportProgress> progressHandler, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                Logger.Debug($@"Export query = ""{searchQuery}"", search result limit = {searchResultLimit?.ToString() ?? "none"}, object type = {typeof(T).Name}.");
                return exportFunction(searchFunction(searchQuery, searchResultLimit), progressHandler, cancellationToken);
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

        private Task ScanAsync<T>(string scanDirectory, IProgress<object> progressHandler, int objectsInDatabaseCount,
            Func<List<string>> indexRetrievalFunction, string indexPrefix, Action indexCreationAction, Func<string, T> getObjectByMd5HashFunction)
            where T : LibgenObject
        {
            return Task.Run(() =>
            {
                Logger.Debug($"Scan request in directory = {scanDirectory}, object count = {objectsInDatabaseCount}, object type = {typeof(T).Name}.");
                int found = 0;
                int notFound = 0;
                int errors = 0;
                if (objectsInDatabaseCount > 0)
                {
                    Logger.Debug("Retrieving the list of indexes.");
                    List<string> indexList = indexRetrievalFunction();
                    if (!indexList.Contains(indexPrefix + "Md5Hash"))
                    {
                        Logger.Debug("Creating an index on Md5Hash column.");
                        progressHandler.Report(new GenericProgress(GenericProgress.Event.SCAN_CREATING_INDEXES));
                        indexCreationAction();
                    }
                    ScanDirectory(scanDirectory.ToLower(), scanDirectory, progressHandler, getObjectByMd5HashFunction, ref found, ref notFound, ref errors);
                }
                progressHandler.Report(new ScanCompleteProgress(found, notFound, errors));
            });
        }

        private static bool HasEbookExtension(string source)
        {
            return (source.EndsWith(".djvu") || source.EndsWith(".mobi") || source.EndsWith(".pdf") || source.EndsWith(".epub")|| source.EndsWith(".doc") || source.EndsWith(".docx")  );
        }

        private void ScanDirectory<T>(string rootScanDirectory, string scanDirectory, IProgress<object> progressHandler,
            Func<string, T> getObjectByMd5HashFunction, ref int found, ref int notFound, ref int errors)
            where T : LibgenObject
        {
            try
            {
                foreach (string filePath in Directory.EnumerateFiles(scanDirectory))
                {
                    string relativeFilePath = filePath;
                    if (relativeFilePath.ToLower().StartsWith(rootScanDirectory))
                    {
                        relativeFilePath = relativeFilePath.Substring(rootScanDirectory.Length + 1);
                    }

                    if (!HasEbookExtension(relativeFilePath))
                    {
                        progressHandler.Report(new ScanUnknownProgress(relativeFilePath, false, ErrorTypes.ERROR_NONE));
                        notFound++;
                        continue;
                    }

                    string md5Hash;
                    try
                    {
                        using (var md5 = MD5.Create())
                        using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 8192, true))
                        {
                            if (fileStream.Length <= 0)
                            {
                                progressHandler.Report(new ScanUnknownProgress(relativeFilePath, true, ErrorTypes.ERROR_FILE_SIZE_ZERO));
                                errors++;
                                continue;
                            }
                            var md5HashArray = md5.ComputeHash(fileStream);
                            md5Hash = BitConverter.ToString(md5HashArray).Replace("-", "").ToLowerInvariant();
                        }
                    }
                    catch (IOException ex)
                    {
                        int hresult = GetHRForException(ex);
                        const int E_PATHTOOLONG = unchecked((int) 0x800700CE);
                        const int E_FILENOTFOUND = unchecked((int) 0x80070002);
                        const int E_SHARING_VIOLATION = unchecked((int) 0x80070020);

                        switch (hresult)
                        {
                            case E_PATHTOOLONG:
                                Logger.Debug($"Error: {nameof(ErrorTypes.ERROR_FILE_PATH_TOO_LONG).Replace("_", " ")}, length: {filePath.Length}");
                                Logger.Exception(ex);
                                progressHandler.Report(new ScanUnknownProgress(relativeFilePath, true, ErrorTypes.ERROR_FILE_PATH_TOO_LONG));
                                errors++;
                                continue;
                            case E_FILENOTFOUND:
                                Logger.Debug($"Error: {nameof(ErrorTypes.ERROR_FILE_NOT_FOUND).Replace("_", " ")}, filePath: {filePath}");
                                Logger.Exception(ex);
                                progressHandler.Report(new ScanUnknownProgress(relativeFilePath, true, ErrorTypes.ERROR_FILE_NOT_FOUND));
                                errors++;
                                continue;
                            case E_SHARING_VIOLATION:
                                Logger.Debug($"Error: {nameof(ErrorTypes.ERROR_FILE_ACCESS).Replace("_", " ")}, filePath: {filePath}");
                                Logger.Exception(ex);
                                progressHandler.Report(new ScanUnknownProgress(relativeFilePath, true, ErrorTypes.ERROR_FILE_IN_USE));
                                errors++;
                                continue;
                        }

                        Logger.Debug($"Error: {nameof(ErrorTypes.ERROR_IO_EXCEPTION).Replace("_", " ")}, IO-EXCEPTION: {ex}");
                        Logger.Exception(ex);
                        progressHandler.Report(new ScanUnknownProgress(relativeFilePath, true, ErrorTypes.ERROR_IO_EXCEPTION));
                        errors++;
                        continue;
                    }
                    catch (Exception exception)
                    {
                        Logger.Debug($"Couldn't calculate MD5 hash for the file: {filePath}");
                        Logger.Exception(exception);
                        progressHandler.Report(new ScanUnknownProgress(relativeFilePath, true, ErrorTypes.ERROR_OTHER));
                        errors++;
                        continue;
                    }
                    try
                    {
                        var libgenObject = getObjectByMd5HashFunction(md5Hash);
                        if (libgenObject != null)
                        {
                            progressHandler.Report(new ScanProgress<T>(relativeFilePath, libgenObject));
                            found++;
                        }
                        else
                        {
                            progressHandler.Report(new ScanUnknownProgress(relativeFilePath, false, ErrorTypes.ERROR_NONE));
                            notFound++;
                        }
                    }
                    catch (Exception exception)
                    {
                        Logger.Debug($"Error: {nameof(ErrorTypes.ERROR_MD5_HASH_NOT_IN_DB).Replace("_", " ")}, MD5 hash: {md5Hash} in the database for the file: {filePath}");
                        Logger.Exception(exception);
                        progressHandler.Report(new ScanUnknownProgress(relativeFilePath, true, ErrorTypes.ERROR_MD5_HASH_NOT_IN_DB));
                        errors++;
                    }
                }
                foreach (string directoryPath in Directory.EnumerateDirectories(scanDirectory))
                {
                    ScanDirectory(rootScanDirectory, directoryPath, progressHandler, getObjectByMd5HashFunction, ref found, ref notFound, ref errors);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.Debug($"Error: {nameof(ErrorTypes.ERROR_DIRECTORY_ACCESS).Replace("_", " ")}, filePath: {scanDirectory}");
                Logger.Exception(ex);
                progressHandler.Report(new ScanUnknownProgress(scanDirectory, true, ErrorTypes.ERROR_DIRECTORY_ACCESS));
                errors++;
            }
            catch (IOException ex)
            {
                int hresult = GetHRForException(ex);
                const int E_DIRECTORYNOTFOUND = unchecked((int)0x80070003);

                if (hresult == E_DIRECTORYNOTFOUND)
                {
                    Logger.Debug($"Error: {nameof(ErrorTypes.ERROR_DIRECTORY_NOT_FOUND).Replace("_", " ")}, filePath: {scanDirectory}");
                    Logger.Exception(ex);
                    progressHandler.Report(new ScanUnknownProgress(scanDirectory, true, ErrorTypes.ERROR_FILE_PATH_TOO_LONG));
                    errors++;
                }
            }
            catch (Exception exception)
            {
                Logger.Exception(exception);
                progressHandler.Report(new ScanUnknownProgress(scanDirectory, true, ErrorTypes.ERROR_OTHER));
                errors++;
            }
        }

        private static int GetHRForException(Exception exception)
        {
            if (exception == null) throw new ArgumentNullException();

            //on first call there is possible pollution of thread IErrorInfo with sensitive data
            int hr = Marshal.GetHRForException(exception);
            //therefore call with empty ex. obj. to cleanup IErrorInfo
            Marshal.GetHRForException(new Exception());
            return hr;
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

        private bool CheckIfIndexExists(List<string> existingIndexes, string prefix, string fieldName)
        {
            return existingIndexes.Contains(prefix + fieldName);
        }

        private void CheckAndCreateIndex(List<string> existingIndexes, string prefix, string fieldName, IProgress<object> progressHandler,
            Action createIndexAction)
        {
            if (!CheckIfIndexExists(existingIndexes, prefix, fieldName))
            {
                Logger.Debug($"Index on {fieldName} doesn't exist, creating it.");
                if (progressHandler != null)
                {
                    progressHandler.Report(new ImportCreateIndexProgress(fieldName));
                }
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

        private long? GetFreeSpaceOnDatabaseDiskDrive()
        {
            return FileUtils.GetFreeSpaceForDiskByPath(Path.GetDirectoryName(localDatabase.DatabaseFullPath));
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
            mirrorSettings.ArticlesMirrorName =
                ValidateAndCorrectSelectedMirror(mirrorSettings.ArticlesMirrorName, mirror => mirror.SciMagDownloadUrl);
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

        private void ValidateAndCorrectDirectoryPaths()
        {
            string downloadDirectory = AppSettings.Download.DownloadDirectory;
            if (String.IsNullOrWhiteSpace(downloadDirectory) || !Directory.Exists(downloadDirectory))
            {
                AppSettings.Download.DownloadDirectory = Path.Combine(Environment.AppDataDirectory, DEFAULT_DOWNLOAD_DIRECTORY_NAME);
            }
        }

        private void ApplicationUpdateCheck(object sender, Updater.UpdateCheckEventArgs e)
        {
            LastApplicationUpdateCheckResult = e.Result;
            ApplicationUpdateCheckCompleted?.Invoke(this, EventArgs.Empty);
            LastApplicationUpdateCheckDateTime = DateTime.Now;
            if (e.Result == null)
            {
                AppSettings.LastUpdate.LastCheckedAt = LastApplicationUpdateCheckDateTime;
                SaveSettings();
            }
            ConfigureUpdater();
        }
    }
}
