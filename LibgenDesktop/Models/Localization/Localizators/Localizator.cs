using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LibgenDesktop.Models.Localization.Localizators
{
    internal abstract class Localizator
    {
        private readonly List<Translation> prioritizedTranslationList;

        public Localizator(List<Translation> prioritizedTranslationList, LanguageFormatter formatter)
        {
            this.prioritizedTranslationList = prioritizedTranslationList;
            Formatter = formatter;
        }

        protected LanguageFormatter Formatter { get; }

        protected string Format(Func<Translation, string> translationField, object templateArguments = null)
        {
            foreach (Translation translation in prioritizedTranslationList)
            {
                string result = translationField(translation)?.Replace("{new-line}", "\r\n");
                if (result != null)
                {
                    return templateArguments == null ? result : RenderTemplate(result, templateArguments);
                }
            }
            return "Error";
        }

        private string RenderTemplate(string template, object templateArguments)
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
