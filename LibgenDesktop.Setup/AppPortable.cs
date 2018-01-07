using System;
using System.IO;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;

namespace LibgenDesktop.Setup
{
    internal static class AppPortable
    {
        public static void Build32BitPortablePackage()
        {
            BuildPortablePackage(false);
        }

        public static void Build64BitPortablePackage()
        {
            BuildPortablePackage(true);
        }

        private static void BuildPortablePackage(bool is64Bit)
        {
            using (ZipArchive zipArchive = ZipArchive.Create())
            {
                foreach (string fileName in AppFiles.GetFileList(is64Bit))
                {
                    zipArchive.AddEntry(Path.GetFileName(fileName), Utils.GetFullFilePath(AppFiles.GetBinariesDirectoryPath(is64Bit), fileName));
                }
                string outputFilePath = Utils.GetFullFilePath(@"..\Release", String.Format(Constants.PORTABLE_PACKAGE_FILE_NAME_FORMAT, is64Bit ? 64 : 32));
                zipArchive.SaveTo(outputFilePath, CompressionType.Deflate);
            }
        }
    }
}
