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
                case "1.2":
                    return MigrateFrom_1_2(localDatabase, databaseMetadata);
                case "1.2.1":
                    return MigrateFrom_1_2_1(localDatabase, databaseMetadata);
                case Constants.CURRENT_DATABASE_VERSION:
                    return true;
                default:
                    return false;
            }
        }

        public static bool UsesOldFictionSchema(DatabaseMetadata databaseMetadata)
        {
            Version metadataVersion = databaseMetadata.GetParsedVersion();
            if (metadataVersion == null)
            {
                throw new Exception("Database metadata has an invalid version number.");
            }
            return metadataVersion < new Version("1.4");
        }

        private static bool MigrateFrom_0_7(LocalDatabase localDatabase, DatabaseMetadata databaseMetadata)
        {
            return AddFilesTable(localDatabase) && RecreateFictionTables(localDatabase) && AddFileIdColumns(localDatabase) &&
                UpdateMetadata(localDatabase, databaseMetadata);
        }

        private static bool MigrateFrom_1_0(LocalDatabase localDatabase, DatabaseMetadata databaseMetadata)
        {
            return AddFilesTable(localDatabase) && RecreateFictionTables(localDatabase) && AddFileIdColumns(localDatabase) &&
                UpdateMetadata(localDatabase, databaseMetadata);
        }

        private static bool MigrateFrom_1_2(LocalDatabase localDatabase, DatabaseMetadata databaseMetadata)
        {
            return RecreateFictionTables(localDatabase) && UpdateMetadata(localDatabase, databaseMetadata);
        }

        private static bool MigrateFrom_1_2_1(LocalDatabase localDatabase, DatabaseMetadata databaseMetadata)
        {
            return RecreateFictionTables(localDatabase) && UpdateMetadata(localDatabase, databaseMetadata);
        }

        private static bool UpdateMetadata(LocalDatabase localDatabase, DatabaseMetadata databaseMetadata)
        {
            try
            {
                databaseMetadata.AppName = Constants.DATABASE_METADATA_APP_NAME;
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

        private static bool AddFilesTable(LocalDatabase localDatabase)
        {
            try
            {
                localDatabase.CreateFilesTable();
                return true;
            }
            catch (Exception exception)
            {
                Logger.Exception(exception);
                return false;
            }
        }

        private static bool AddFileIdColumns(LocalDatabase localDatabase)
        {
            try
            {
                localDatabase.AddFileIdColumnToNonFictionTable();
                localDatabase.AddFileIdColumnToSciMagTable();
                return true;
            }
            catch (Exception exception)
            {
                Logger.Exception(exception);
                return false;
            }
        }

        private static bool RecreateFictionTables(LocalDatabase localDatabase)
        {
            try
            {
                localDatabase.DropFictionTables();
                localDatabase.CreateFictionTables();
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
