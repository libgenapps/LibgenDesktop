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
            DATA_NOT_FOUND,
            EXCEPTION
        }

        private const double IMPORT_PROGRESS_UPDATE_INTERVAL = 0.5;
        private const double SEARCH_PROGRESS_UPDATE_INTERVAL = 0.1;

        private readonly string settingsFilePath;
        private LocalDatabase localDatabase;

        public MainModel()
        {
            AppDataDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
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
            Mirrors = MirrorStorage.LoadMirrors(Path.Combine(AppDataDirectory, MIRRORS_FILE_NAME));
        }

        public AppSettings AppSettings { get; }
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
                        switch (tableType)
                        {
                            case TableType.NON_FICTION:
                                if (NonFictionBookCount != 0)
                                {
                                    throw new Exception("Update-импорт пока не поддерживается.");
                                }
                                ImportObjects(sqlDumpReader, progressHandler, cancellationToken, TableDefinitions.NonFiction,
                                    parsedTableDefinition, localDatabase.AddNonFictionBooks);
                                UpdateNonFictionBookCount();
                                break;
                            case TableType.FICTION:
                                if (FictionBookCount != 0)
                                {
                                    throw new Exception("Update-импорт пока не поддерживается.");
                                }
                                ImportObjects(sqlDumpReader, progressHandler, cancellationToken, TableDefinitions.Fiction,
                                    parsedTableDefinition, localDatabase.AddFictionBooks);
                                UpdateFictionBookCount();
                                break;
                            case TableType.SCI_MAG:
                                if (SciMagArticleCount != 0)
                                {
                                    throw new Exception("Update-импорт пока не поддерживается.");
                                }
                                ImportObjects(sqlDumpReader, progressHandler, cancellationToken, TableDefinitions.SciMag,
                                    parsedTableDefinition, localDatabase.AddSciMagArticles);
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

        private void ImportObjects<T>(SqlDumpReader sqlDumpReader, IProgress<object> progressHandler, CancellationToken cancellationToken,
            TableDefinition<T> tableDefinition, SqlDumpReader.ParsedTableDefinition parsedTableDefinition, Action<List<T>> databaseBatchImportAction)
            where T : new()
        {
            DateTime lastUpdateDateTime = DateTime.Now;
            int importedObjectCount = 0;
            List<T> currentBatchObjects = new List<T>(INSERT_TRANSACTION_BATCH);
            List<Action<T, string>> sortedColumnSetters = tableDefinition.GetSortedColumnSetters(parsedTableDefinition.Columns.Select(column => column.ColumnName));
            foreach (T importingObject in sqlDumpReader.ParseImportObjects(sortedColumnSetters))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
                currentBatchObjects.Add(importingObject);
                importedObjectCount++;
                if (currentBatchObjects.Count == INSERT_TRANSACTION_BATCH)
                {
                    databaseBatchImportAction(currentBatchObjects);
                    currentBatchObjects.Clear();
                    DateTime now = DateTime.Now;
                    if ((now - lastUpdateDateTime).TotalSeconds > IMPORT_PROGRESS_UPDATE_INTERVAL)
                    {
                        progressHandler.Report(new ImportObjectsProgress(importedObjectCount));
                        lastUpdateDateTime = now;
                    }
                }
            }
            if (currentBatchObjects.Any())
            {
                databaseBatchImportAction(currentBatchObjects);
            }
            progressHandler.Report(new ImportObjectsProgress(importedObjectCount));
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
