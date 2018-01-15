using System;
using System.Collections.Generic;
using LibgenDesktop.Models.Database;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.SqlDump;

namespace LibgenDesktop.Models.Import
{
    internal class FictionImporter : Importer<FictionBook>
    {
        private readonly DateTime lastModifiedDateTime;
        private readonly int lastModifiedLibgenId;

        public FictionImporter(LocalDatabase localDatabase, bool isUpdateMode)
            : base(localDatabase, isUpdateMode, TableDefinitions.Fiction)
        {
            if (isUpdateMode)
            {
                FictionBook lastModifiedFictionBook = LocalDatabase.GetLastModifiedFictionBook();
                lastModifiedDateTime = lastModifiedFictionBook.LastModifiedDateTime;
                lastModifiedLibgenId = lastModifiedFictionBook.LibgenId;
            }
        }

        protected override void InsertBatch(List<FictionBook> objectBatch)
        {
            LocalDatabase.AddFictionBooks(objectBatch);
        }

        protected override void UpdateBatch(List<FictionBook> objectBatch)
        {
            LocalDatabase.UpdateFictionBooks(objectBatch);
        }

        protected override bool IsNewObject(FictionBook importingObject)
        {
            return importingObject.LastModifiedDateTime > lastModifiedDateTime ||
                (importingObject.LastModifiedDateTime == lastModifiedDateTime && importingObject.LibgenId != lastModifiedLibgenId);
        }

        protected override int? GetExistingObjectIdByLibgenId(int libgenId)
        {
            return LocalDatabase.GetFictionBookIdByLibgenId(libgenId);
        }
    }
}
