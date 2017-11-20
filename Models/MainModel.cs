using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibgenDesktop.Models.Database;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.ProgressArgs;
using LibgenDesktop.Models.Settings;
using LibgenDesktop.Models.SqlDump;
using LibgenDesktop.Models.Utils;
using static LibgenDesktop.Common.Constants;

namespace LibgenDesktop.Models
{
    internal class MainModel
    {
        private readonly LocalDatabase localDatabase;

        public MainModel()
        {
            AppSettings = SettingsStorage.LoadSettings();
            localDatabase = new LocalDatabase(AppSettings.DatabaseFileName);
            AllBooks = new AsyncBookCollection();
            SearchResults = new AsyncBookCollection();
        }

        public AppSettings AppSettings { get; }
        public AsyncBookCollection AllBooks { get; }
        public AsyncBookCollection SearchResults { get; }

        public Task LoadAllBooksAsync(IProgress<LoadAllBooksProgress> progressHandler, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                int totalBookCount = localDatabase.CountBooks();
                AllBooks.SetCapacity(totalBookCount);
                int currentBatchBookNumber = 0;
                int reportProgressBatchSize = totalBookCount / 1000;
                foreach (Book book in localDatabase.GetAllBooks())
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }
                    AllBooks.AddBook(book);
                    currentBatchBookNumber++;
                    if (currentBatchBookNumber == reportProgressBatchSize)
                    {
                        progressHandler.Report(new LoadAllBooksProgress(AllBooks.AddedBookCount, totalBookCount));
                        currentBatchBookNumber = 0;
                        if (AllBooks.AddedBookCount - AllBooks.Count > AllBooks.Count)
                        {
                            AllBooks.UpdateReportedBookCount();
                        }
                    }
                }
                if (currentBatchBookNumber > 0)
                {
                    progressHandler.Report(new LoadAllBooksProgress(AllBooks.AddedBookCount, totalBookCount, isFinished: true));
                }
                AllBooks.UpdateReportedBookCount();
            });
        }

        public void ClearSearchResults()
        {
            SearchResults.Clear();
        }

        public Task SearchBooksAsync(string searchQuery, IProgress<SearchBooksProgress> progressHandler, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                int currentBatchBookNumber = 0;
                foreach (Book book in localDatabase.SearchBooks(searchQuery))
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }
                    SearchResults.AddBook(book);
                    currentBatchBookNumber++;
                    if (currentBatchBookNumber == SEARCH_REPORT_PROGRESS_BATCH_SIZE)
                    {
                        progressHandler.Report(new SearchBooksProgress(SearchResults.AddedBookCount));
                        currentBatchBookNumber = 0;
                        if (SearchResults.AddedBookCount - SearchResults.Count > SearchResults.Count)
                        {
                            SearchResults.UpdateReportedBookCount();
                        }
                    }
                }
                if (currentBatchBookNumber > 0)
                {
                    progressHandler.Report(new SearchBooksProgress(SearchResults.AddedBookCount, isFinished: true));
                }
                SearchResults.UpdateReportedBookCount();
            });
        }

        public Task<Book> LoadBookAsync(int bookId)
        {
            return Task<Book>.Run(() =>
            {
                return localDatabase.GetBookById(bookId);
            });
        }

        public Task ImportSqlDumpAsync(string sqlDumpFilePath, IProgress<ImportSqlDumpProgress> progressHandler, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                using (SqlDumpReader sqlDumpReader = new SqlDumpReader(sqlDumpFilePath))
                {
                    EventHandler<SqlDumpReader.ReadRowsProgressEventArgs> readRowsProgressHandler = (sender, e) =>
                    {
                        progressHandler.Report(new ImportSqlDumpProgress(e.RowsParsed, e.CurrentPosition, e.TotalLength));
                    };
                    sqlDumpReader.ReadRowsProgress += readRowsProgressHandler;
                    List<Book> currentBatchBooks = new List<Book>(INSERT_TRANSACTION_BATCH);
                    foreach (Book book in sqlDumpReader.ReadRows())
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            break;
                        }
                        currentBatchBooks.Add(book);
                        if (currentBatchBooks.Count == INSERT_TRANSACTION_BATCH)
                        {
                            localDatabase.AddBooks(currentBatchBooks);
                            foreach (Book currentBatchBook in currentBatchBooks)
                            {
                                currentBatchBook.ExtendedProperties = null;
                            }
                            AllBooks.AddBooks(currentBatchBooks);
                            if (AllBooks.Count == 0)
                            {
                                AllBooks.UpdateReportedBookCount();
                            }
                            currentBatchBooks.Clear();
                        }
                    }
                    if (currentBatchBooks.Any())
                    {
                        localDatabase.AddBooks(currentBatchBooks);
                        foreach (Book currentBatchBook in currentBatchBooks)
                        {
                            currentBatchBook.ExtendedProperties = null;
                        }
                        AllBooks.AddBooks(currentBatchBooks);
                    }
                    sqlDumpReader.ReadRowsProgress -= readRowsProgressHandler;
                }
                AllBooks.UpdateReportedBookCount();
            });
        }

        public void SaveSettings()
        {
            SettingsStorage.SaveSettings(AppSettings);
        }
    }
}
