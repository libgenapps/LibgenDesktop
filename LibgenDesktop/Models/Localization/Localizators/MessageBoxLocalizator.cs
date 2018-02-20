using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators
{
    internal class MessageBoxLocalizator : Localizator
    {
        public MessageBoxLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter)
        {
            Ok = Format(translation => translation?.Ok);
            Yes = Format(translation => translation?.Yes);
            No = Format(translation => translation?.No);
        }

        public string Ok { get; }
        public string Yes { get; }
        public string No { get; }

        private string Format(Func<Translation.MessageBoxTranslation, string> field)
        {
            return Format(translation => field(translation?.MessageBox));
        }
    }
}
