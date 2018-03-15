using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators
{
    internal class AboutWindowLocalizator : Localizator
    {
        public AboutWindowLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter)
        {
            WindowTitle = Format(translation => translation?.WindowTitle);
            ApplicationName = Format(translation => translation?.ApplicationName);
            CheckForUpdates = Format(translation => translation?.CheckForUpdates);
            CheckingUpdates = Format(translation => translation?.CheckingUpdates);
            OfflineModeIsOnTooltip = Format(translation => translation?.OfflineModeIsOnTooltip);
            LatestVersion = Format(translation => translation?.LatestVersion);
            Update = Format(translation => translation?.Update);
            Translators = Format(translation => translation?.Translators);
        }

        public string WindowTitle { get; }
        public string ApplicationName { get; }
        public string CheckForUpdates { get; }
        public string CheckingUpdates { get; }
        public string OfflineModeIsOnTooltip { get; }
        public string LatestVersion { get; }
        public string Update { get; }
        public string Translators { get; }

        public string GetVersionString(string version, DateTime date) =>
            Format(translation => translation?.Version, new { version, date = Formatter.ToFormattedDateString(date) });

        public string GetNewVersionAvailableString(string version, DateTime date) =>
            Format(translation => translation?.NewVersionAvailable, new { version, date = Formatter.ToFormattedDateString(date) });

        private string Format(Func<Translation.AboutTranslation, string> field, object templateArguments = null)
        {
            return Format(translation => field(translation?.About), templateArguments);
        }
    }
}
