using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators
{
    internal class ErrorWindowLocalizator : Localizator
    {
        public ErrorWindowLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter)
        {
            WindowTitle = Format(translation => translation?.WindowTitle);
            UnexpectedError = Format(translation => translation?.UnexpectedError) + ":";
            Copy = Format(translation => translation?.Copy);
            Close = Format(translation => translation?.Close);
        }

        public string WindowTitle { get; }
        public string UnexpectedError { get; }
        public string Copy { get; }
        public string Close { get; }

        private string Format(Func<Translation.ErrorWindowTranslation, string> field)
        {
            return Format(translation => field(translation?.ErrorWindow));
        }
    }
}
