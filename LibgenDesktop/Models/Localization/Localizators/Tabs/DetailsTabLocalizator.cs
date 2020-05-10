using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators.Tabs
{
    internal abstract class DetailsTabLocalizator<T> : Localizator<Translation.DetailsTabsTranslation> where T : class
    {
        private readonly Func<Translation, T> detailsTabTranslationSectionSelector;

        public DetailsTabLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter,
            Func<Translation, T> detailsTabTranslationSectionSelector)
            : base(prioritizedTranslationList, formatter, translation => translation?.DetailsTabs)
        {
            this.detailsTabTranslationSectionSelector = detailsTabTranslationSectionSelector;
            CopyContextMenu = Format(section => section?.CopyContextMenu).Replace("{text}", "{0}");
            Close = Format(section => section?.Close);
            Yes = Format(section => section?.Yes);
            No = Format(section => section?.No);
            Unknown = Format(section => section?.Unknown);
            Portrait = Format(section => section?.Portrait);
            Landscape = Format(section => section?.Landscape);
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

        protected string Format(Func<T, string> detailsTabTranslationSectionFieldSelector, object templateArguments = null) =>
            Format(translation => detailsTabTranslationSectionFieldSelector(detailsTabTranslationSectionSelector(translation)), templateArguments);

        protected string FormatHeader(Func<T, string> detailsTabTranslationSectionFieldSelector) =>
            Format(detailsTabTranslationSectionFieldSelector) + ":";

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
