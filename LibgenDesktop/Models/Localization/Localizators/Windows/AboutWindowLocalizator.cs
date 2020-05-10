using System;
using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization.Localizators.Windows
{
    internal class AboutWindowLocalizator : Localizator<Translation.AboutTranslation>
    {
        public AboutWindowLocalizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
            : base(prioritizedTranslationList, formatter, translation => translation?.About)
        {
            WindowTitle = Format(section => section?.WindowTitle);
            ApplicationName = Format(section => section?.ApplicationName);
            CheckForUpdates = Format(section => section?.CheckForUpdates);
            CheckingUpdates = Format(section => section?.CheckingUpdates);
            OfflineModeIsOnTooltip = Format(section => section?.OfflineModeIsOnTooltip);
            LatestVersion = Format(section => section?.LatestVersion);
            Update = Format(section => section?.Update);
            Translators = Format(section => section?.Translators);
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
            Format(section => section?.Version, new { version, date = Formatter.ToFormattedDateString(date) });

        public string GetNewVersionAvailableString(string version, DateTime date) =>
            Format(section => section?.NewVersionAvailable, new { version, date = Formatter.ToFormattedDateString(date) });
    }
}
