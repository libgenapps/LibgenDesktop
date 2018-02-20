using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators
{
    internal abstract class ExporterLocalizator : Localizator
    {
        public ExporterLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter)
        {
            Yes = Format(translation => translation?.Yes);
            No = Format(translation => translation?.No);
            Unknown = Format(translation => translation?.Unknown);
            Portrait = Format(translation => translation?.Portrait);
            Landscape = Format(translation => translation?.Landscape);
        }

        protected string Yes { get; }
        protected string No { get; }
        protected string Unknown { get; }
        protected string Portrait { get; }
        protected string Landscape { get; }

        protected string StringBooleanToYesNoUnknownString(string value)
        {
            return StringBooleanToLabelString(value, Yes, No, Unknown);
        }

        protected string StringBooleanToOrientationString(string value)
        {
            return StringBooleanToLabelString(value, Portrait, Landscape, Unknown);
        }

        protected string Format(Func<Translation.ExporterTranslation, string> field)
        {
            return Format(translation => field(translation?.Exporter));
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
