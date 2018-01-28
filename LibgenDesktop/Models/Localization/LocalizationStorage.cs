using System.Collections.Generic;

namespace LibgenDesktop.Models.Localization
{
    internal static class LocalizationStorage
    {
        public static Dictionary<string, string> LoadLanguages()
        {
            return new Dictionary<string, string>
            {
                { "Russian", "Russian (русский)" }
            };
        }
    }
}
