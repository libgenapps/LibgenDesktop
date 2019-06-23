using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators
{
    internal class SqlDebuggerWindowLocalizator : Localizator
    {
        public SqlDebuggerWindowLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter)
        {
            WindowTitle = Format(translation => translation?.WindowTitle);
            SqlQueryTextBoxHeader = Format(translation => translation?.SqlQueryTextBoxHeader);
            Copy = Format(translation => translation?.Copy);
            Close = Format(translation => translation?.Close);
        }

        public string WindowTitle { get; }
        public string SqlQueryTextBoxHeader { get; }
        public string Copy { get; }
        public string Close { get; }

        private string Format(Func<Translation.SqlDebuggerWindowTranslation, string> field)
        {
            return Format(translation => field(translation?.SqlDebugger));
        }
    }
}
