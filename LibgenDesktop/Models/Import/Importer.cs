using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LibgenDesktop.Models.Database;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.ProgressArgs;
using LibgenDesktop.Models.SqlDump;
using static LibgenDesktop.Common.Constants;

namespace LibgenDesktop.Models.Import
{
    internal abstract class Importer
    {
        public abstract void Import(SqlDumpReader sqlDumpReader, IProgress<object> progressHandler, CancellationToken cancellationToken,
            SqlDumpReader.ParsedTableDefinition parsedTableDefinition);
    }

    internal abstract class Importer<T> : Importer where T : LibgenObject, new()
    {
        private const double IMPORT_PROGRESS_UPDATE_INTERVAL = 0.5;

        private readonly bool isUpdateMode;
        private readonly TableDefinition<T> tableDefinition;
        private readonly List<T> currentBatchObjectsToInsert;
        private readonly List<T> currentBatchObjectsToUpdate;

        protected Importer(LocalDatabase localDatabase, bool isUpdateMode, TableDefinition<T> tableDefinition)
        {
            LocalDatabase = localDatabase;
            this.isUpdateMode = isUpdateMode;
            this.tableDefinition = tableDefinition;
            currentBatchObjectsToInsert = new List<T>(DATABASE_TRANSACTION_BATCH);
            currentBatchObjectsToUpdate = new List<T>(DATABASE_TRANSACTION_BATCH);
        }

        protected LocalDatabase LocalDatabase { get; }

        public override void Import(SqlDumpReader sqlDumpReader, IProgress<object> progressHandler, CancellationToken cancellationToken,
            SqlDumpReader.ParsedTableDefinition parsedTableDefinition)
        {
            List<Action<T, string>> sortedColumnSetters =
                tableDefinition.GetSortedColumnSetters(parsedTableDefinition.Columns.Select(column => column.ColumnName));
            Import(sqlDumpReader.ParseImportObjects(sortedColumnSetters), progressHandler, cancellationToken);
        }

        public void Import(IEnumerable<T> importingObjects, IProgress<object> progressHandler, CancellationToken cancellationToken)
        {
            DateTime lastProgressUpdateDateTime = DateTime.Now;
            int addedObjectCount = 0;
            int updatedObjectCount = 0;
            currentBatchObjectsToInsert.Clear();
            currentBatchObjectsToUpdate.Clear();
            ReportProgress(progressHandler, addedObjectCount, updatedObjectCount);
            foreach (T importingObject in importingObjects)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
                if (!isUpdateMode)
                {
                    currentBatchObjectsToInsert.Add(importingObject);
                }
                else if (IsNewObject(importingObject))
                {
                    int? existingObjectId = GetExistingObjectIdByLibgenId(importingObject.LibgenId);
                    if (existingObjectId.HasValue)
                    {
                        importingObject.Id = existingObjectId.Value;
                        currentBatchObjectsToUpdate.Add(importingObject);
                    }
                    else
                    {
                        currentBatchObjectsToInsert.Add(importingObject);
                    }
                }
                if (currentBatchObjectsToInsert.Count + currentBatchObjectsToUpdate.Count == DATABASE_TRANSACTION_BATCH)
                {
                    InsertBatch(currentBatchObjectsToInsert);
                    addedObjectCount += currentBatchObjectsToInsert.Count;
                    if (cancellationToken.IsCancellationRequested)
                    {
                        ReportProgress(progressHandler, addedObjectCount, updatedObjectCount);
                        return;
                    }
                    currentBatchObjectsToInsert.Clear();
                    UpdateBatch(currentBatchObjectsToUpdate);
                    updatedObjectCount += currentBatchObjectsToUpdate.Count;
                    if (cancellationToken.IsCancellationRequested)
                    {
                        ReportProgress(progressHandler, addedObjectCount, updatedObjectCount);
                        return;
                    }
                    currentBatchObjectsToUpdate.Clear();
                    DateTime now = DateTime.Now;
                    if ((now - lastProgressUpdateDateTime).TotalSeconds > IMPORT_PROGRESS_UPDATE_INTERVAL)
                    {
                        ReportProgress(progressHandler, addedObjectCount, updatedObjectCount);
                        lastProgressUpdateDateTime = now;
                    }
                }
            }
            if (currentBatchObjectsToInsert.Any())
            {
                InsertBatch(currentBatchObjectsToInsert);
                addedObjectCount += currentBatchObjectsToInsert.Count;
            }
            if (currentBatchObjectsToUpdate.Any())
            {
                UpdateBatch(currentBatchObjectsToUpdate);
                updatedObjectCount += currentBatchObjectsToUpdate.Count;
            }
            ReportProgress(progressHandler, addedObjectCount, updatedObjectCount);
        }

        protected abstract void InsertBatch(List<T> objectBatch);
        protected abstract void UpdateBatch(List<T> objectBatch);
        protected abstract bool IsNewObject(T importingObject);
        protected abstract int? GetExistingObjectIdByLibgenId(int libgenId);

        private void ReportProgress(IProgress<object> progressHandler, int objectsAdded, int objectsUpdated)
        {
            progressHandler.Report(new ImportObjectsProgress(objectsAdded, objectsUpdated));
        }
    }
}
