using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators.Windows
{
    internal class DatabaseErrorWindowLocalizator : Localizator<Translation.DatabaseErrorTranslation>
    {
        public DatabaseErrorWindowLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.DatabaseError)
        {
            WindowTitle = Format(section => section?.WindowTitle);
            OpenAnotherDatabase = Format(section => section?.OpenAnotherDatabase);
            StartSetupWizard = Format(section => section?.StartSetupWizard);
            DeleteFiction = Format(section => section?.DeleteFiction);
            Exit = Format(section => section?.Exit);
            Ok = Format(section => section?.Ok);
            Cancel = Format(section => section?.Cancel);
        }

        public string WindowTitle { get; }
        public string OpenAnotherDatabase { get; }
        public string StartSetupWizard { get; }
        public string DeleteFiction { get; }
        public string Exit { get; }
        public string Ok { get; }
        public string Cancel { get; }

        public string GetDatabaseNotFoundText(string database) => Format(section => section?.DatabaseNotFound, new { database });

        public string GetDatabaseNotValidText(string file) => Format(section => section?.DatabaseNotValid, new { file });
        
        public string GetDatabaseDumpFileText(string file) => Format(section => section?.DatabaseDumpFile, new { file });

        public string GetLibgenServerDatabaseText(string database) => Format(section => section?.LibgenServerDatabase, new { database });

        public string GetOldFictionSchemaText(string database) => Format(section => section?.OldFictionSchema, new { database });
        
        public string GetHeader(string error) => Format(section => section?.HeaderTemplate, new { error });
    }
}
