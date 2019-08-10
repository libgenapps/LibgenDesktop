using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using LibgenDesktop.Common;

namespace LibgenDesktop.Models.Localization
{
    internal class LanguageFormatter
    {
        private readonly List<Translation> prioritizedTranslationList;
        private readonly CultureInfo cultureInfo;
        private readonly NumberFormatInfo numberFormatInfo;
        private readonly string dateFormat;
        private readonly string timeFormat;
        private readonly string dateTimeFormat;
        private readonly string[] fileSizePostfixes;

        public LanguageFormatter(List<Translation> prioritizedTranslationList)
        {
            if (!prioritizedTranslationList.Any())
            {
                throw new Exception("No available translations.");
            }
            this.prioritizedTranslationList = prioritizedTranslationList;
            try
            {
                cultureInfo = new CultureInfo(prioritizedTranslationList.First().General.CultureCode);
            }
            catch (Exception exception)
            {
                Logger.Exception(exception);
                cultureInfo = CultureInfo.InvariantCulture;
            }
            numberFormatInfo = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            numberFormatInfo.NumberDecimalSeparator = GetTranslationFieldValue(translation => translation?.DecimalSeparator);
            numberFormatInfo.NumberGroupSeparator = GetTranslationFieldValue(translation => translation?.ThousandsSeparator);
            dateFormat = GetTranslationFieldValue(translation => translation?.DateFormat);
            timeFormat = GetTranslationFieldValue(translation => translation?.TimeFormat);
            dateTimeFormat = dateFormat + " " + timeFormat;
            fileSizePostfixes = new[]
            {
                GetTranslationFieldValue(translation => translation?.FileSizePostfixes?.Byte),
                GetTranslationFieldValue(translation => translation?.FileSizePostfixes?.Kilobyte),
                GetTranslationFieldValue(translation => translation?.FileSizePostfixes?.Megabyte),
                GetTranslationFieldValue(translation => translation?.FileSizePostfixes?.Gigabyte),
                GetTranslationFieldValue(translation => translation?.FileSizePostfixes?.Terabyte)
            };
        }

        public string ToFormattedString(int value)
        {
            return value.ToString("N0", numberFormatInfo);
        }

        public string ToFormattedString(long value)
        {
            return value.ToString("N0", numberFormatInfo);
        }

        public string ToFormattedString(decimal value)
        {
            return value.ToString(numberFormatInfo);
        }

        public string ToFormattedDateString(DateTime dateTime)
        {
            return dateTime.ToString(dateFormat, cultureInfo);
        }

        public string ToFormattedTimeString(DateTime dateTime)
        {
            return dateTime.ToString(timeFormat, cultureInfo);
        }

        public string ToFormattedDateTimeString(DateTime dateTime)
        {
            return dateTime.ToString(dateTimeFormat, cultureInfo);
        }

        public string FileSizeToString(long fileSize, bool showBytes)
        {
            int postfixIndex = fileSize != 0 ? (int)Math.Floor(Math.Log(fileSize) / Math.Log(1024)) : 0;
            StringBuilder resultBuilder = new StringBuilder();
            resultBuilder.Append((fileSize / Math.Pow(1024, postfixIndex)).ToString("N2", numberFormatInfo));
            resultBuilder.Append(" ");
            resultBuilder.Append(fileSizePostfixes[postfixIndex]);
            if (showBytes && postfixIndex != 0)
            {
                resultBuilder.Append(" (");
                resultBuilder.Append(fileSize.ToString("N0", numberFormatInfo));
                resultBuilder.Append(" ");
                resultBuilder.Append(fileSizePostfixes[0]);
                resultBuilder.Append(")");
            }
            return resultBuilder.ToString();
        }

        private string GetTranslationFieldValue(Func<Translation.FormattingInfo, string> translationField)
        {
            foreach (Translation translation in prioritizedTranslationList)
            {
                Translation.FormattingInfo formattingInfo = translation?.Formatting;
                if (formattingInfo != null)
                {
                    string result = translationField(formattingInfo);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            throw new Exception("Could not find the requested translation field.");
        }
    }
}
