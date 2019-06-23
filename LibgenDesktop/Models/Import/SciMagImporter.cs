using System;
using System.Collections;
using System.Collections.Generic;
using LibgenDesktop.Models.Database;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.SqlDump;

namespace LibgenDesktop.Models.Import
{
    internal class SciMagImporter : Importer<SciMagArticle>
    {
        public SciMagImporter(LocalDatabase localDatabase, BitArray existingLibgenIds)
            : base(localDatabase, existingLibgenIds, TableDefinitions.SciMag)
        {
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
            return false;
        }

        protected override int? GetExistingObjectIdByLibgenId(int libgenId)
        {
            return null;
        }
    }
}
