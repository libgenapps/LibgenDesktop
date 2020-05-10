using System;
using System.Collections.Generic;
using System.Reflection;

namespace LibgenDesktop.Models.Localization.Localizators
{
    internal abstract class Localizator<T> where T : class
    {
        private readonly List<Translation> prioritizedTranslationList;
        private readonly Func<Translation, T> translationSectionSelector;

        public Localizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter, Func<Translation, T> translationSectionSelector)
        {
            this.prioritizedTranslationList = prioritizedTranslationList;
            Formatter = formatter;
            this.translationSectionSelector = translationSectionSelector;
        }

        protected LanguageFormatter Formatter { get; }

        protected string Format(Func<T, string> translationSectionField, object templateArguments = null)
        {
            return Format(translation => translationSectionField(translationSectionSelector(translation)), templateArguments);
        }

        protected string Format(Func<Translation, string> translationFieldSelector, object templateArguments = null)
        {
            foreach (Translation translation in prioritizedTranslationList)
            {
                string result = translationFieldSelector(translation)?.Replace("{new-line}", "\r\n");
                if (result != null)
                {
                    return templateArguments == null ? result : RenderTemplate(result, templateArguments);
                }
            }
            return "Error";
        }

        private static string RenderTemplate(string template, object templateArguments)
        {
            string result = template;
            foreach (PropertyInfo property in templateArguments.GetType().GetProperties())
            {
                result = result.Replace("{" + property.Name + "}", property.GetValue(templateArguments).ToString());
            }
            return result;
        }
    }
}
