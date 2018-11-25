using System;
using LibgenDesktop.Common;
using LibgenDesktop.Models.Entities;

namespace LibgenDesktop.Models.Database
{
    internal static class Migration
    {
        public static bool Migrate(LocalDatabase localDatabase, DatabaseMetadata databaseMetadata)
        {
            switch (databaseMetadata.Version)
            {
                case "0.7":
                    return MigrateFrom_0_7(localDatabase, databaseMetadata);
                case "1.0":
                    return MigrateFrom_1_0(localDatabase, databaseMetadata);
                case Constants.CURRENT_DATABASE_VERSION:
                    return true;
                default:
                    return false;
            }
        }

        private static bool MigrateFrom_0_7(LocalDatabase localDatabase, DatabaseMetadata databaseMetadata)
        {
            return AddFiles(localDatabase) && UpdateMetadataVersion(localDatabase, databaseMetadata);
        }

        private static bool MigrateFrom_1_0(LocalDatabase localDatabase, DatabaseMetadata databaseMetadata)
        {
            return AddFiles(localDatabase) && UpdateMetadataVersion(localDatabase, databaseMetadata);
        }

        private static bool UpdateMetadataVersion(LocalDatabase localDatabase, DatabaseMetadata databaseMetadata)
        {
            try
            {
                databaseMetadata.Version = Constants.CURRENT_DATABASE_VERSION;
                localDatabase.UpdateMetadata(databaseMetadata);
                return true;
            }
            catch (Exception exception)
            {
                Logger.Exception(exception);
                return false;
            }
        }

        private static bool AddFiles(LocalDatabase localDatabase)
        {
            try
            {
                localDatabase.CreateFilesTable();
                localDatabase.AddFileIdColumns();
                return true;
            }
            catch (Exception exception)
            {
                Logger.Exception(exception);
                return false;
            }
        }
    }
}
