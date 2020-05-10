using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using LibgenDesktop.Common;
using LibgenDesktop.Models.Utils;
using Newtonsoft.Json;
using static LibgenDesktop.Common.Constants;

namespace LibgenDesktop.Models.Localization
{
    internal class LocalizationStorage
    {
        internal class TranslationPropertyInfo
        {
            public TranslationPropertyInfo(PropertyInfo propertyInfo)
            {
                PropertyInfo = propertyInfo;
                IsSection = false;
                SectionProperties = null;
            }

            public TranslationPropertyInfo(PropertyInfo propertyInfo, List<TranslationPropertyInfo> sectionProperties)
            {
                PropertyInfo = propertyInfo;
                IsSection = true;
                SectionProperties = sectionProperties;
            }

            public PropertyInfo PropertyInfo { get; }
            public bool IsSection { get; }
            public List<TranslationPropertyInfo> SectionProperties { get; }
        }

        private static List<TranslationPropertyInfo> translationProperties;
        private static int totalTranslationPropertyCount;

        public LocalizationStorage(string languageDirectoryPath, string selectedLanguageName)
        {
            LoadLanguages(languageDirectoryPath, selectedLanguageName);
        }

        public List<Language> Languages { get; private set; }
        public Language CurrentLanguage { get; private set; }

        public event EventHandler LanguageChanged;

        public void SwitchLanguage(string newLanguageName)
        {
            if (newLanguageName != null && newLanguageName != CurrentLanguage.Name)
            {
                Language newLanguage = Languages.FirstOrDefault(language => language.Name == newLanguageName);
                if (newLanguage != null)
                {
                    CurrentLanguage = newLanguage;
                    LanguageChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private static Translation LoadTranslation(string filePath)
        {
            JsonSerializer jsonSerializer = new JsonSerializer();
            using (StreamReader streamReader = new StreamReader(filePath))
            using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
            {
                return jsonSerializer.Deserialize<Translation>(jsonTextReader);
            }
        }

        private static Language CreateLanguage(Translation languageTranslation, List<Translation> translationList, Translation defaultTranslation)
        {
            List<Translation> prioritizedTranslationList = new List<Translation> { languageTranslation };
            string translationCultureCode = languageTranslation.General?.CultureCode ?? String.Empty;
            if (translationCultureCode.Length > 2)
            {
                string languageCode = translationCultureCode.Substring(0, 2);
                Translation baseLanguageTranslation = translationList.FirstOrDefault(translation => (translation != languageTranslation) &&
                    (translation.General?.CultureCode?.StartsWith(languageCode, StringComparison.OrdinalIgnoreCase) ?? false));
                if (baseLanguageTranslation != null)
                {
                    prioritizedTranslationList.Add(baseLanguageTranslation);
                }
            }
            if (defaultTranslation != null)
            {
                prioritizedTranslationList.Add(defaultTranslation);
            }
            return new Language(prioritizedTranslationList, CalculatePercentTranslated(languageTranslation));
        }


        private void LoadLanguages(string languageDirectoryPath, string selectedLanguageName)
        {
            Languages = new List<Language>();
            List<Translation> translations = new List<Translation>();
            foreach (string translationFilePath in Directory.EnumerateFiles(languageDirectoryPath, "*.lng"))
            {
                try
                {
                    translations.Add(LoadTranslation(translationFilePath));
                }
                catch (Exception exception)
                {
                    Logger.Debug($"Error while trying to load language file: {translationFilePath}");
                    Logger.Exception(exception);
                }
            }
            if (!translations.Any())
            {
                throw new Exception($"Cannot find language files in {languageDirectoryPath}");
            }
            Translation defaultTranslation = translations.FirstOrDefault(translation =>
                translation.General.Name.CompareOrdinalIgnoreCase(DEFAULT_LANGUAGE_NAME));
            Language defaultLanguage;
            if (defaultTranslation != null)
            {
                defaultLanguage = CreateLanguage(defaultTranslation, translations, null);
                Languages.Add(defaultLanguage);
            }
            else
            {
                defaultLanguage = null;
            }
            foreach (Translation translation in translations)
            {
                if (translation == defaultTranslation)
                {
                    continue;
                }
                Languages.Add(CreateLanguage(translation, translations, defaultTranslation));
            }
            Languages = Languages.OrderBy(language => language.Name).ToList();
            Language selectedLanguage = Languages.FirstOrDefault(language => language.Name.CompareOrdinalIgnoreCase(selectedLanguageName));
            if (selectedLanguage != null)
            {
                CurrentLanguage = selectedLanguage;
            }
            else
            {
                string currentCultureCode = CultureInfo.CurrentUICulture.Name;
                Language currentCultureLanguage = Languages.FirstOrDefault(language => language.CultureCode.CompareOrdinalIgnoreCase(currentCultureCode));
                if (currentCultureLanguage != null)
                {
                    CurrentLanguage = currentCultureLanguage;
                }
                else
                {
                    string currentLanguageCode = currentCultureCode.Substring(0, 2);
                    Language currentBaseCultureLanguage = Languages.FirstOrDefault(language =>
                        language.CultureCode.StartsWith(currentLanguageCode, StringComparison.OrdinalIgnoreCase));
                    if (currentBaseCultureLanguage != null)
                    {
                        CurrentLanguage = currentBaseCultureLanguage;
                    }
                    else
                    {
                        if (defaultLanguage != null)
                        {
                            CurrentLanguage = defaultLanguage;
                        }
                        else
                        {
                            CurrentLanguage = Languages.First();
                        }
                    }
                }
            }
        }

        private static decimal CalculatePercentTranslated(Translation translation)
        {
            if (translationProperties == null)
            {
                totalTranslationPropertyCount = 0;
                translationProperties = RetrievePropertyInfos(typeof(Translation), ref totalTranslationPropertyCount);
            }
            int translatedPropertyCount = GetTranslatedPropertyCount(translationProperties, translation);
            return ((decimal)translatedPropertyCount * 100) / totalTranslationPropertyCount;
        }

        private static int GetTranslatedPropertyCount(List<TranslationPropertyInfo> translationSectionPropertyInfos, object translationSection)
        {
            int result = 0;
            foreach (TranslationPropertyInfo translationPropertyInfo in translationSectionPropertyInfos)
            {
                object propertyValue = translationSection != null ? translationPropertyInfo.PropertyInfo.GetValue(translationSection) : null;
                if (!translationPropertyInfo.IsSection)
                {
                    if (propertyValue != null)
                    {
                        result++;
                    }
                }
                else
                {
                    result += GetTranslatedPropertyCount(translationPropertyInfo.SectionProperties, propertyValue);
                }
            }
            return result;
        }

        private static List<TranslationPropertyInfo> RetrievePropertyInfos(Type type, ref int totalPropertyCount)
        {
            List<TranslationPropertyInfo> result = new List<TranslationPropertyInfo>();
            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                TranslationPropertyInfo translationPropertyInfo;
                if (propertyInfo.PropertyType == typeof(String))
                {
                    translationPropertyInfo = new TranslationPropertyInfo(propertyInfo);
                    totalPropertyCount++;
                }
                else
                {
                    translationPropertyInfo =
                        new TranslationPropertyInfo(propertyInfo, RetrievePropertyInfos(propertyInfo.PropertyType, ref totalPropertyCount));
                }
                result.Add(translationPropertyInfo);
            }
            return result;
        }
    }
}
