using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators.Windows
{
    internal class SqlDebuggerWindowLocalizator : Localizator<Translation.SqlDebuggerWindowTranslation>
    {
        public SqlDebuggerWindowLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.SqlDebugger)
        {
            WindowTitle = Format(section => section?.WindowTitle);
            SqlQueryTextBoxHeader = Format(section => section?.SqlQueryTextBoxHeader);
            Copy = Format(section => section?.Copy);
            Close = Format(section => section?.Close);
        }

        public string WindowTitle { get; }
        public string SqlQueryTextBoxHeader { get; }
        public string Copy { get; }
        public string Close { get; }
    }
}
