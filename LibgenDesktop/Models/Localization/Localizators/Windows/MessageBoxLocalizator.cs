using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators.Windows
{
    internal class MessageBoxLocalizator : Localizator<Translation.MessageBoxTranslation>
    {
        public MessageBoxLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.MessageBox)
        {
            Ok = Format(section => section?.Ok);
            Yes = Format(section => section?.Yes);
            No = Format(section => section?.No);
        }

        public string Ok { get; }
        public string Yes { get; }
        public string No { get; }
    }
}
