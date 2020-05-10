using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators.Export
{
    internal abstract class ExporterLocalizator<T> : Localizator<Translation.ExporterTranslation> where T : class
    {
        private readonly Func<Translation, T> exporterTranslationSectionSelector;

        public ExporterLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter,
            Func<Translation, T> exporterTranslationSectionSelector)
            : base(prioritizedTranslationList, formatter, translation => translation?.Exporter)
        {
            this.exporterTranslationSectionSelector = exporterTranslationSectionSelector;
            Yes = Format(section => section?.Yes);
            No = Format(section => section?.No);
            Unknown = Format(section => section?.Unknown);
            Portrait = Format(section => section?.Portrait);
            Landscape = Format(section => section?.Landscape);
        }

        protected string Yes { get; }
        protected string No { get; }
        protected string Unknown { get; }
        protected string Portrait { get; }
        protected string Landscape { get; }

        protected string StringBooleanToYesNoUnknownString(string value) => StringBooleanToLabelString(value, Yes, No, Unknown);

        protected string StringBooleanToOrientationString(string value) => StringBooleanToLabelString(value, Portrait, Landscape, Unknown);

        protected string Format(Func<T, string> exporterTranslationSectionFieldSelector, object templateArguments = null) =>
            Format(translation => exporterTranslationSectionFieldSelector(exporterTranslationSectionSelector(translation)), templateArguments);

        private static string StringBooleanToLabelString(string value, string value1Label, string value0Label, string valueUnknownLabel)
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
