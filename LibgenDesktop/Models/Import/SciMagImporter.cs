using System;
using System.Collections.Generic;
using LibgenDesktop.Models.Database;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.SqlDump;

namespace LibgenDesktop.Models.Import
{
    internal class SciMagImporter : Importer<SciMagArticle>
    {
        private readonly DateTime lastAddedDateTime;
        private readonly int lastModifiedLibgenId;

        public SciMagImporter(LocalDatabase localDatabase, bool isUpdateMode)
            : base(localDatabase, isUpdateMode, TableDefinitions.SciMag)
        {
            if (isUpdateMode)
            {
                SciMagArticle lastAddedSciMagArticle = LocalDatabase.GetLastAddedSciMagArticle();
                lastAddedDateTime = lastAddedSciMagArticle.AddedDateTime ?? DateTime.MinValue;
                lastModifiedLibgenId = lastAddedSciMagArticle.LibgenId;
            }
        }

        protected override void InsertBatch(List<SciMagArticle> objectBatch)
        {
            LocalDatabase.AddSciMagArticles(objectBatch);
        }

        protected override void UpdateBatch(List<SciMagArticle> objectBatch)
        {
            throw new Exception("Updating is not supported for articles.");
        }

        protected override bool IsNewObject(SciMagArticle importingObject)
        {
            return importingObject.AddedDateTime > lastAddedDateTime ||
                (importingObject.AddedDateTime == lastAddedDateTime && importingObject.LibgenId != lastModifiedLibgenId);
        }

        protected override int? GetExistingObjectIdByLibgenId(int libgenId)
        {
            return null;
        }
    }
}
