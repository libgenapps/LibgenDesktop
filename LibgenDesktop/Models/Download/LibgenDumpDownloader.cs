using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LibgenDesktop.Common;

namespace LibgenDesktop.Models.Download
{
    internal class LibgenDumpDownloader
    {
        internal enum RoundedSizeUnit
        {
            BYTES,
            KILOBYTES,
            MEGABYTES,
            GIGABYTES,
            TERABYTES
        }

        internal class DumpMetadata
        {
            public string Url { get; set; }
            public string FileName { get; set; }
            public DateTime LastModified { get; set; }
            public decimal RoundedSize { get; set; }
            public RoundedSizeUnit RoundedSizeUnit { get; set; }
        }

        internal class Dumps
        {
            public DumpMetadata NonFiction { get; set; }
            public DumpMetadata Fiction { get; set; }
            public DumpMetadata SciMag { get; set; }
        }

        internal enum LoadAndParseStatus
        {
            COMPLETED = 1,
            LOAD_FAILED,
            PARSE_FAILED,
            CANCELLED
        }

        internal class LoadAndParseResult
        {
            public LoadAndParseStatus Status { get; set; }
            public Dumps Dumps { get; set; }
        }

        private readonly string databaseDumpPageUrl;
        private readonly string databaseDumpPageTransformationName;
        private HttpClient httpClient;

        public LibgenDumpDownloader(string databaseDumpPageUrl, string databaseDumpPageTransformationName)
        {
            this.databaseDumpPageUrl = databaseDumpPageUrl;
            this.databaseDumpPageTransformationName = databaseDumpPageTransformationName;
        }

        public void Configure(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<LoadAndParseResult> LoadAndParseDumpPageAsync(CancellationToken cancellationToken)
        {
            LoadAndParseResult result = new LoadAndParseResult();
            DownloadUtils.DownloadPageResult downloadPageResult = await DownloadUtils.DownloadPageAsync(httpClient, databaseDumpPageUrl, cancellationToken);
            if (downloadPageResult.DownloadResult == DownloadUtils.DownloadResult.CANCELLED)
            {
                result.Status = LoadAndParseStatus.CANCELLED;
                return result;
            }
            if ((downloadPageResult.DownloadResult == DownloadUtils.DownloadResult.ERROR) || (downloadPageResult.HttpStatusCode != HttpStatusCode.OK))
            {
                result.Status = LoadAndParseStatus.LOAD_FAILED;
                return result;
            }
            string dumpList;
            try
            {
                dumpList = DownloadUtils.ExecuteTransformation(downloadPageResult.PageContent, databaseDumpPageTransformationName, htmlDecode: false);
            }
            catch (Exception exception)
            {
                Logger.Debug($"Transformation {databaseDumpPageTransformationName} threw an exception.");
                Logger.Exception(exception);
                result.Status = LoadAndParseStatus.PARSE_FAILED;
                return result;
            }
            try
            {
                result.Dumps = ParseDumps(dumpList);
            }
            catch (Exception exception)
            {
                Logger.Debug($"Dump metadata parser threw an exception.");
                Logger.Exception(exception);
                result.Status = LoadAndParseStatus.PARSE_FAILED;
                return result;
            }
            result.Status = LoadAndParseStatus.COMPLETED;
            return result;
        }

        public Task<DownloadUtils.DownloadResult> DownloadDumpAsync(string dumpUrl, string dumpFilePath, IProgress<object> progressHandler,
            CancellationToken cancellationToken)
        {
            return DownloadUtils.DownloadFileAsync(httpClient, dumpUrl, dumpFilePath, true, progressHandler, cancellationToken);
        }

        private static decimal ParseRoundedSize(string fileSizeString)
        {
            if (String.IsNullOrWhiteSpace(fileSizeString))
            {
                return 0;
            }
            if (!Decimal.TryParse(fileSizeString.Substring(0, fileSizeString.Length - 1), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture,
                out decimal result))
            {
                return 0;
            }
            return result;
        }

        private static RoundedSizeUnit ParseRoundedSizeUnit(string fileSizeString)
        {
            if (String.IsNullOrWhiteSpace(fileSizeString))
            {
                return RoundedSizeUnit.BYTES;
            }
            char unit = fileSizeString.Last();
            switch (unit)
            {
                case 'K':
                    return RoundedSizeUnit.KILOBYTES;
                case 'M':
                    return RoundedSizeUnit.MEGABYTES;
                case 'G':
                    return RoundedSizeUnit.GIGABYTES;
                case 'T':
                    return RoundedSizeUnit.TERABYTES;
                default:
                    return RoundedSizeUnit.BYTES;
            }
        }

        private static Dumps ParseDumps(string dumpList)
        {
            Logger.Debug($"Parsing dump list:\r\n{dumpList}");
            List<string> dumpListLines = new List<string>(3);
            using (StringReader stringReader = new StringReader(dumpList))
            {
                string line;
                do
                {
                    line = stringReader.ReadLine();
                    if (line != null)
                    {
                        dumpListLines.Add(line);
                    }
                }
                while (line != null);
            }
            if (dumpListLines.Count < 3)
            {
                throw new Exception($"Expected at least 3 dump list lines but got {dumpListLines.Count}.");
            }
            Dumps result = new Dumps()
            {
                NonFiction = ParseDumpMetadata(dumpListLines[0]),
                Fiction = ParseDumpMetadata(dumpListLines[1]),
                SciMag = ParseDumpMetadata(dumpListLines[2])
            };
            return result;
        }

        private static DumpMetadata ParseDumpMetadata(string dumpListLine)
        {
            string[] dumpListLineFields = dumpListLine.Split(new[] { '|' });
            if (dumpListLineFields.Length != 4)
            {
                throw new Exception($"Expected at least 4 dump line fields but got {dumpListLineFields.Length}.");
            }
            DumpMetadata result = new DumpMetadata
            {
                Url = dumpListLineFields[0],
                FileName = dumpListLineFields[1],
                LastModified = DateTime.ParseExact(dumpListLineFields[2], "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                RoundedSize = ParseRoundedSize(dumpListLineFields[3]),
                RoundedSizeUnit = ParseRoundedSizeUnit(dumpListLineFields[3])
            };
            return result;
        }
    }
}
