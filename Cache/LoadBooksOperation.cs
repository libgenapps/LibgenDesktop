using System.Collections.Generic;
using System.Threading;
using LibgenDesktop.Database;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Interface;

namespace LibgenDesktop.Cache
{
    internal class LoadBooksOperation : ProgressOperation
    {
        private readonly LocalDatabase localDatabase;
        private readonly List<Book> targetList;

        public LoadBooksOperation(LocalDatabase localDatabase, List<Book> targetList)
        {
            this.localDatabase = localDatabase;
            this.targetList = targetList;
        }

        protected override void DoWork(CancellationToken token)
        {
            int totalBookCount = localDatabase.CountBooks();
            int currentBatchBookNumber = 0;
            int reportProgressBatchSize = totalBookCount / 1000;
            foreach (Book book in localDatabase.GetAllBooks())
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                targetList.Add(book);
                currentBatchBookNumber++;
                if (currentBatchBookNumber == reportProgressBatchSize)
                {
                    RaiseProgressEvent(new ProgressEventArgs
                    {
                        ProgressDescription = $"Загрузка книг (загружено {targetList.Count.ToString("N0", Formatters.ThousandsSeparatedNumberFormat)} из {totalBookCount.ToString("N0", Formatters.ThousandsSeparatedNumberFormat)})...",
                        PercentCompleted = (double)targetList.Count * 100 / totalBookCount
                    });
                    currentBatchBookNumber = 0;
                }
            }
            if (currentBatchBookNumber > 0)
            {
                RaiseProgressEvent(new ProgressEventArgs
                {
                    PercentCompleted = 100
                });
            }
            RaiseCompletedEvent();
        }
    }
}
