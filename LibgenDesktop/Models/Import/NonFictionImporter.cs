using System;
using System.Collections;
using System.Collections.Generic;
using LibgenDesktop.Models.Database;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.SqlDump;

namespace LibgenDesktop.Models.Import
{
    internal class NonFictionImporter : Importer<NonFictionBook>
    {
        private readonly DateTime lastModifiedDateTime;
        private readonly int lastModifiedLibgenId;

        public NonFictionImporter(LocalDatabase localDatabase, BitArray existingLibgenIds)
            : base(localDatabase, existingLibgenIds, TableDefinitions.NonFiction)
        {
            if (IsUpdateMode)
            {
                NonFictionBook lastModifiedNonFictionBook = LocalDatabase.GetLastModifiedNonFictionBook();
                lastModifiedDateTime = lastModifiedNonFictionBook.LastModifiedDateTime;
                lastModifiedLibgenId = lastModifiedNonFictionBook.LibgenId;
            }
        }

        public NonFictionImporter(LocalDatabase localDatabase, BitArray existingLibgenIds, NonFictionBook lastModifiedNonFictionBook)
            : base(localDatabase, existingLibgenIds, TableDefinitions.NonFiction)
        {
            lastModifiedDateTime = lastModifiedNonFictionBook.LastModifiedDateTime;
            lastModifiedLibgenId = lastModifiedNonFictionBook.LibgenId;
        }

        protected override void InsertBatch(List<NonFictionBook> objectBatch)
        {
            LocalDatabase.AddNonFictionBooks(objectBatch);
        }

        protected override void UpdateBatch(List<NonFictionBook> objectBatch)
        {
            LocalDatabase.UpdateNonFictionBooks(objectBatch);
        }

        protected override bool IsNewObject(NonFictionBook importingObject)
        {
            return importingObject.LastModifiedDateTime > lastModifiedDateTime ||
                (importingObject.LastModifiedDateTime == lastModifiedDateTime && importingObject.LibgenId != lastModifiedLibgenId);
        }

        protected override int? GetExistingObjectIdByLibgenId(int libgenId)
        {
            return LocalDatabase.GetNonFictionBookIdByLibgenId(libgenId);
        }
    }
}
