using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators.Windows
{
    internal class DatabaseWindowLocalizator : Localizator<Translation.DatabaseTranslation>
    {
        public DatabaseWindowLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.Database)
        {
            WindowTitle = Format(section => section?.WindowTitle);
            CurrentDatabase = Format(section => section?.CurrentDatabase);
            NonFiction = Format(section => section?.NonFiction);
            Fiction = Format(section => section?.Fiction);
            SciMagArticles = Format(section => section?.SciMagArticles);
            TotalBooks = Format(section => section?.TotalBooks) + ":";
            TotalArticles = Format(section => section?.TotalArticles) + ":";
            LastUpdate = Format(section => section?.LastUpdate) + ":";
            Never = Format(section => section?.Never);
            IndexesRequiredTitle = Format(section => section?.IndexesRequiredTitle);
            IndexesRequiredText = Format(section => section?.IndexesRequiredText);
            CreatingIndexes = Format(section => section?.CreatingIndexes);
            ChangeDatabase = Format(section => section?.ChangeDatabase);
            BrowseDatabaseDialogTitle = Format(section => section?.BrowseDatabaseDialogTitle);
            Databases = Format(section => section?.Databases);
            AllFiles = Format(section => section?.AllFiles);
            Error = Format(section => section?.Error);
            OldFictionSchemaTitle = Format(section => section?.OldFictionSchemaTitle);
            Close = Format(section => section?.Close);
        }

        public string WindowTitle { get; }
        public string CurrentDatabase { get; }
        public string NonFiction { get; }
        public string Fiction { get; }
        public string SciMagArticles { get; }
        public string TotalBooks { get; }
        public string TotalArticles { get; }
        public string LastUpdate { get; }
        public string Never { get; }
        public string IndexesRequiredTitle { get; }
        public string IndexesRequiredText { get; }
        public string CreatingIndexes { get; }
        public string ChangeDatabase { get; }
        public string BrowseDatabaseDialogTitle { get; }
        public string Databases { get; }
        public string AllFiles { get; }
        public string Error { get; }
        public string OldFictionSchemaTitle { get; }
        public string Close { get; }

        public string GetDatabaseNotValidText(string file) => Format(section => section?.DatabaseNotValid, new { file });

        public string GetDatabaseDumpFileText(string file) => Format(section => section?.DatabaseDumpFile, new { file });

        public string GetLibgenServerDatabaseText(string database) => Format(section => section?.LibgenServerDatabase, new { database });

        public string GetOldFictionSchemaText(string database) => Format(section => section?.OldFictionSchemaText, new { database });
    }
}
