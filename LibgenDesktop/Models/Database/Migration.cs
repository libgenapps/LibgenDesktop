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
                case Constants.CURRENT_DATABASE_VERSION:
                    return true;
                default:
                    return false;
            }
        }

        private static bool MigrateFrom_0_7(LocalDatabase localDatabase, DatabaseMetadata databaseMetadata)
        {
            databaseMetadata.Version = Constants.CURRENT_DATABASE_VERSION;
            localDatabase.UpdateMetadata(databaseMetadata);
            return true;
        }
    }
}
