using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using LibgenDesktop.Database;
using LibgenDesktop.Infrastructure;
using LibgenDesktop.Interface;

namespace LibgenDesktop.Import
{
    internal class ImportSqlDumpOperation : ProgressOperation
    {
        private readonly LocalDatabase localDatabase;
        private readonly string sqlDumpFilePath;
        private readonly List<Book> targetList;

        public ImportSqlDumpOperation(LocalDatabase localDatabase, string sqlDumpFilePath, List<Book> targetList)
        {
            this.localDatabase = localDatabase;
            this.sqlDumpFilePath = sqlDumpFilePath;
            this.targetList = targetList;
        }

        public override string Title => "Импорт из SQL-дампа";

        protected override void DoWork(CancellationToken token)
        {
            RaiseProgressEvent(new ProgressEventArgs
            {
                ProgressDescription = "Импорт из SQL-дампа...",
                PercentCompleted = 0
            });
            using (SqlDumpReader sqlDumpReader = new SqlDumpReader(sqlDumpFilePath))
            {
                sqlDumpReader.ReadRowsProgress += SqlDumpReader_ReadRowsProgress;
                List<Book> currentBatchBooks = new List<Book>(LocalDatabase.INSERT_TRANSACTION_BATCH);
                foreach (Book book in sqlDumpReader.ReadRows())
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                    currentBatchBooks.Add(book);
                    if (currentBatchBooks.Count == LocalDatabase.INSERT_TRANSACTION_BATCH)
                    {
                        localDatabase.AddBooks(currentBatchBooks);
                        foreach (Book currentBatchBook in currentBatchBooks)
                        {
                            currentBatchBook.ExtendedProperties = null;
                        }
                        targetList.AddRange(currentBatchBooks);
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
                    targetList.AddRange(currentBatchBooks);
                }
                sqlDumpReader.ReadRowsProgress -= SqlDumpReader_ReadRowsProgress;
            }
            RaiseCompletedEvent();
        }

        private void SqlDumpReader_ReadRowsProgress(object sender, SqlDumpReader.ReadRowsProgressEventArgs e)
        {
            RaiseProgressEvent(new ProgressEventArgs
            {
                ProgressDescription = $"Импорт из SQL-дампа... (импортировано {e.RowsParsed.ToString("N0", Formatters.ThousandsSeparatedNumberFormat)} книг)",
                PercentCompleted = ((double)e.CurrentPosition * 100 / e.TotalLength)
            });
        }
    }
}
