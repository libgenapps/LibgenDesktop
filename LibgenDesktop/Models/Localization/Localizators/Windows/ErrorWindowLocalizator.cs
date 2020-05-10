using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators.Windows
{
    internal class ErrorWindowLocalizator : Localizator<Translation.ErrorWindowTranslation>
    {
        public ErrorWindowLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.ErrorWindow)
        {
            WindowTitle = Format(section => section?.WindowTitle);
            UnexpectedError = Format(section => section?.UnexpectedError) + ":";
            Copy = Format(section => section?.Copy);
            Close = Format(section => section?.Close);
        }

        public string WindowTitle { get; }
        public string UnexpectedError { get; }
        public string Copy { get; }
        public string Close { get; }
    }
}
