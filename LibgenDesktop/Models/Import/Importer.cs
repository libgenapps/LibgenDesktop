using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LibgenDesktop.Models.Database;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.SqlDump;
using static LibgenDesktop.Common.Constants;

namespace LibgenDesktop.Models.Import
{
    internal abstract class Importer
    {
        internal delegate void ImportProgressReporter(int objectsAdded, int objectsUpdated);

        internal class ImportResult
        {
            public ImportResult(int addedObjectCount, int updatedObjectCount)
            {
                AddedObjectCount = addedObjectCount;
                UpdatedObjectCount = updatedObjectCount;
            }

            public int AddedObjectCount { get; }
            public int UpdatedObjectCount { get; }
        }

        public abstract ImportResult Import(SqlDumpReader sqlDumpReader, ImportProgressReporter progressReporter, double progressUpdateInterval,
            CancellationToken cancellationToken, SqlDumpReader.ParsedTableDefinition parsedTableDefinition);
    }

    internal abstract class Importer<T> : Importer where T : LibgenObject, new()
    {
        private readonly BitArray existingLibgenIds;
        private readonly TableDefinition<T> tableDefinition;
        private readonly List<T> currentBatchObjectsToInsert;
        private readonly List<T> currentBatchObjectsToUpdate;

        protected Importer(LocalDatabase localDatabase, BitArray existingLibgenIds, TableDefinition<T> tableDefinition)
        {
            LocalDatabase = localDatabase;
            this.existingLibgenIds = existingLibgenIds;
            IsUpdateMode = existingLibgenIds != null && existingLibgenIds.Length > 0;
            this.tableDefinition = tableDefinition;
            currentBatchObjectsToInsert = new List<T>(DATABASE_TRANSACTION_BATCH);
            currentBatchObjectsToUpdate = new List<T>(DATABASE_TRANSACTION_BATCH);
        }

        protected LocalDatabase LocalDatabase { get; }
        protected bool IsUpdateMode { get; }

        public override ImportResult Import(SqlDumpReader sqlDumpReader, ImportProgressReporter progressReporter, double progressUpdateInterval,
            CancellationToken cancellationToken, SqlDumpReader.ParsedTableDefinition parsedTableDefinition)
        {
            List<Action<T, string>> sortedColumnSetters =
                tableDefinition.GetSortedColumnSetters(parsedTableDefinition.Columns.Select(column => column.ColumnName));
            return Import(sqlDumpReader.ParseImportObjects(sortedColumnSetters), progressReporter, progressUpdateInterval, cancellationToken);
        }

        public ImportResult Import(IEnumerable<T> importingObjects, ImportProgressReporter progressReporter, double progressUpdateInterval,
            CancellationToken cancellationToken)
        {
            DateTime lastProgressUpdateDateTime = DateTime.Now;
            int addedObjectCount = 0;
            int updatedObjectCount = 0;
            currentBatchObjectsToInsert.Clear();
            currentBatchObjectsToUpdate.Clear();
            progressReporter(addedObjectCount, updatedObjectCount);
            foreach (T importingObject in importingObjects)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                if (!IsUpdateMode || existingLibgenIds.Length <= importingObject.LibgenId || !existingLibgenIds[importingObject.LibgenId])
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
                    if (currentBatchObjectsToInsert.Any())
                    {
                        InsertBatch(currentBatchObjectsToInsert);
                        addedObjectCount += currentBatchObjectsToInsert.Count;
                        currentBatchObjectsToInsert.Clear();
                        if (cancellationToken.IsCancellationRequested)
                        {
                            break;
                        }
                    }
                    if (currentBatchObjectsToUpdate.Any())
                    {
                        UpdateBatch(currentBatchObjectsToUpdate);
                        updatedObjectCount += currentBatchObjectsToUpdate.Count;
                        currentBatchObjectsToUpdate.Clear();
                        if (cancellationToken.IsCancellationRequested)
                        {
                            break;
                        }
                    }
                    DateTime now = DateTime.Now;
                    if ((now - lastProgressUpdateDateTime).TotalSeconds > progressUpdateInterval)
                    {
                        progressReporter(addedObjectCount, updatedObjectCount);
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
            progressReporter(addedObjectCount, updatedObjectCount);
            return new ImportResult(addedObjectCount, updatedObjectCount);
        }

        protected abstract void InsertBatch(List<T> objectBatch);
        protected abstract void UpdateBatch(List<T> objectBatch);
        protected abstract bool IsNewObject(T importingObject);
        protected abstract int? GetExistingObjectIdByLibgenId(int libgenId);
    }
}
