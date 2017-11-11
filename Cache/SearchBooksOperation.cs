using System;
using System.Collections.Generic;
using System.Threading;
using LibgenDesktop.Database;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Interface;

namespace LibgenDesktop.Cache
{
    internal class SearchBooksOperation : ProgressOperation
    {
        private const int REPORT_PROGRESS_BATCH_SIZE = 2000;

        private readonly LocalDatabase localDatabase;
        private readonly List<Book> targetList;
        private readonly string searchQuery;

        public SearchBooksOperation(LocalDatabase localDatabase, List<Book> targetList, string searchQuery)
        {
            this.localDatabase = localDatabase;
            this.targetList = targetList;
            this.searchQuery = searchQuery;
        }

        public override bool IsUnbounded => true;

        protected override void DoWork(CancellationToken token)
        {
            int currentBatchBookNumber = 0;
            foreach (Book book in localDatabase.SearchBooks(searchQuery))
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                targetList.Add(book);
                currentBatchBookNumber++;
                if (currentBatchBookNumber == REPORT_PROGRESS_BATCH_SIZE)
                {
                    RaiseProgressEvent(new ProgressEventArgs
                    {
                        ProgressDescription = $"Поиск книг (найдено {targetList.Count.ToString("N0", Formatters.ThousandsSeparatedNumberFormat)})...",
                        PercentCompleted = Double.NaN
                    });
                    currentBatchBookNumber = 0;
                }
            }
            RaiseCompletedEvent();
        }
    }
}
