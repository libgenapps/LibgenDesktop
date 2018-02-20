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
            FirstRunMessage = Format(translation => translation?.FirstRunMessage);
            ChooseOption = Format(translation => translation?.ChooseOption);
            CreateNewDatabase = Format(translation => translation?.CreateNewDatabase);
            OpenExistingDatabase = Format(translation => translation?.OpenExistingDatabase);
            BrowseNewDatabaseDialogTitle = Format(translation => translation?.BrowseNewDatabaseDialogTitle);
            BrowseExistingDatabaseDialogTitle = Format(translation => translation?.BrowseExistingDatabaseDialogTitle);
            Databases = Format(translation => translation?.Databases);
            AllFiles = Format(translation => translation?.AllFiles);
            Error = Format(translation => translation?.Error);
            CannotCreateDatabase = Format(translation => translation?.CannotCreateDatabase);
            Ok = Format(translation => translation?.Ok);
            Cancel = Format(translation => translation?.Cancel);
        }

        public string WindowTitle { get; }
        public string FirstRunMessage { get; }
        public string ChooseOption { get; }
        public string CreateNewDatabase { get; }
        public string OpenExistingDatabase { get; }
        public string BrowseNewDatabaseDialogTitle { get; }
        public string BrowseExistingDatabaseDialogTitle { get; }
        public string Databases { get; }
        public string AllFiles { get; }
        public string Error { get; }
        public string CannotCreateDatabase { get; }
        public string Ok { get; }
        public string Cancel { get; }

        public string GetDatabaseNotFoundText(string database) => Format(translation => translation.DatabaseNotFound, new { database });
        public string GetDatabaseCorruptedText(string database) => Format(translation => translation.DatabaseCorrupted, new { database });

        private string Format(Func<Translation.DatabaseWindowTranslation, string> field, object templateArguments = null)
        {
            return Format(translation => field(translation?.DatabaseWindow), templateArguments);
        }

    }
}
