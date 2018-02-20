using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators
{
    internal abstract class DetailsTabLocalizator : Localizator
    {
        public DetailsTabLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter)
        {
            CopyContextMenu = Format(translation => translation?.CopyContextMenu).Replace("{text}", "{0}");
            Close = Format(translation => translation?.Close);
            Yes = Format(translation => translation?.Yes);
            No = Format(translation => translation?.No);
            Unknown = Format(translation => translation?.Unknown);
            Portrait = Format(translation => translation?.Portrait);
            Landscape = Format(translation => translation?.Landscape);
        }

        public string CopyContextMenu { get; }
        public string Close { get; }
        protected string Yes { get; }
        protected string No { get; }
        protected string Unknown { get; }
        protected string Portrait { get; }
        protected string Landscape { get; }

        protected string StringBooleanToYesNoUnknownString(string value) => StringBooleanToLabelString(value, Yes, No, Unknown);
        protected string StringBooleanToOrientationString(string value) => StringBooleanToLabelString(value, Portrait, Landscape, Unknown);

        protected string Format(Func<Translation.DetailsTabsTranslation, string> field, object templateArguments = null)
        {
            return Format(translation => field(translation?.DetailsTabs), templateArguments);
        }

        private string StringBooleanToLabelString(string value, string value1Label, string value0Label, string valueUnknownLabel)
        {
            switch (value)
            {
                case "0":
                    return value0Label;
                case "1":
                    return value1Label;
                default:
                    return valueUnknownLabel;
            }
        }
    }
}
