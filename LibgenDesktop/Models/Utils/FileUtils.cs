using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

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

        public static long? GetFreeSpaceForDiskByPath(string localOrUncPath)
        {
            bool success = GetDiskFreeSpaceEx(localOrUncPath, out ulong result, out _, out _);
            if (success)
            {
                return (long)result;
            }
            else
            {
                return null;
            }
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetDiskFreeSpaceEx(string lpDirectoryName, out ulong lpFreeBytesAvailable, out ulong lpTotalNumberOfBytes,
            out ulong lpTotalNumberOfFreeBytes);
    }
}
