using System;
using System.IO;
using System.Linq;

namespace LibgenDesktop.Models.Utils
{
    internal static class FileUtils
    {
        public static string RemoveInvalidFileNameCharacters(string fileName, string emptyFileNameReplacement)
        {
            if (String.IsNullOrWhiteSpace(fileName))
            {
                return emptyFileNameReplacement;
            }
            string result = fileName.Trim();
            foreach (char invalidChar in Path.GetInvalidFileNameChars())
            {
                result = result.Replace(invalidChar, '_');
            }
            if (String.IsNullOrEmpty(result) || result.All(c => c == '_'))
            {
                result = emptyFileNameReplacement;
            }
            return result;
        }
    }
}
