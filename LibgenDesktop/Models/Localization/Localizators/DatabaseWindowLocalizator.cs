using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators
{
    internal class DatabaseWindowLocalizator : Localizator
    {
        public DatabaseWindowLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter)
        {
            WindowTitle = Format(translation => translation?.WindowTitle);
            CurrentDatabase = Format(translation => translation?.CurrentDatabase);
            NonFiction = Format(translation => translation?.NonFiction);
            Fiction = Format(translation => translation?.Fiction);
            SciMagArticles = Format(translation => translation?.SciMagArticles);
            TotalBooks = Format(translation => translation?.TotalBooks) + ":";
            TotalArticles = Format(translation => translation?.TotalArticles) + ":";
            LastUpdate = Format(translation => translation?.LastUpdate) + ":";
            Never = Format(translation => translation?.Never);
            IndexesRequiredTitle = Format(translation => translation?.IndexesRequiredTitle);
            IndexesRequiredText = Format(translation => translation?.IndexesRequiredText);
            CreatingIndexes = Format(translation => translation?.CreatingIndexes);
            ChangeDatabase = Format(translation => translation?.ChangeDatabase);
            BrowseDatabaseDialogTitle = Format(translation => translation?.BrowseDatabaseDialogTitle);
            Databases = Format(translation => translation?.Databases);
            AllFiles = Format(translation => translation?.AllFiles);
            Error = Format(translation => translation?.Error);
            Close = Format(translation => translation?.Close);
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
        public string Close { get; }

        public string GetCannotOpenDatabase(string file)
        {
            return Format(translation => translation.CannotOpenDatabase, new { file });
        }

        private string Format(Func<Translation.DatabaseTranslation, string> field, object templateArguments = null)
        {
            return Format(translation => field(translation?.Database), templateArguments);
        }
    }
}
