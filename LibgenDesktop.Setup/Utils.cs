using System.Collections.Generic;
using System.IO;

namespace LibgenDesktop.Setup
{
    internal static class Utils
    {
        public static string GetFullFilePath(string baseDirectory, string relativeFilePath)
        {
            return Path.GetFullPath(Path.Combine(baseDirectory, relativeFilePath));
        }

        public static void MoveFile(string fileName, string targetDirectory)
        {
            string from = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            string to = Path.Combine(targetDirectory, fileName);
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }
            else
            {
                if (File.Exists(to))
                {
                    File.Delete(to);
                }
            }
            File.Move(from, to);
        }

        public static void DeleteTempFiles(params string[] fileNamePatterns)
        {
            foreach (string fileNamePattern in fileNamePatterns)
            {
                foreach (string filePath in Directory.GetFiles(Directory.GetCurrentDirectory(), fileNamePattern))
                {
                    File.Delete(filePath);
                }
            }
        }
    }
}
