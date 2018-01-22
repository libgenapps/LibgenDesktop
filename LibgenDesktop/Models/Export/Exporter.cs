using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using LibgenDesktop.Models.Entities;
using LibgenDesktop.Models.ProgressArgs;
using static LibgenDesktop.Common.Constants;

namespace LibgenDesktop.Models.Export
{
    internal abstract class Exporter<TExportWriter> where TExportWriter : ExportWriter
    {
        private readonly string filePathTemplate;
        private readonly int? rowsPerFile;
        private readonly bool splitIntoMultipleFiles;
        private readonly string fileExtension;
        private readonly Func<string, TExportWriter> exportWriterFactory;

        public Exporter(string filePathTemplate, int? rowsPerFile, bool splitIntoMultipleFiles, string fileExtension,
            Func<string, TExportWriter> exportWriterFactory)
        {
            this.filePathTemplate = filePathTemplate;
            this.rowsPerFile = rowsPerFile;
            this.splitIntoMultipleFiles = splitIntoMultipleFiles;
            this.fileExtension = fileExtension;
            this.exportWriterFactory = exportWriterFactory;
        }

        public ExportResult ExportNonFiction(IEnumerable<NonFictionBook> books, IProgress<ExportProgress> progressHandler, CancellationToken cancellationToken)
        {
            return Export(books, exportWriter => new NonFictionExportObject(exportWriter), progressHandler, cancellationToken);
        }

        public ExportResult ExportFiction(IEnumerable<FictionBook> books, IProgress<ExportProgress> progressHandler, CancellationToken cancellationToken)
        {
            return Export(books, exportWriter => new FictionExportObject(exportWriter), progressHandler, cancellationToken);
        }

        public ExportResult ExportSciMag(IEnumerable<SciMagArticle> articles, IProgress<ExportProgress> progressHandler, CancellationToken cancellationToken)
        {
            return Export(articles, exportWriter => new SciMagExportObject(exportWriter), progressHandler, cancellationToken);
        }

        private ExportResult Export<TLibgenObject, TExportObject>(IEnumerable<TLibgenObject> libgenObjects, Func<TExportWriter, TExportObject> exportObjectFactory,
            IProgress<ExportProgress> progressHandler, CancellationToken cancellationToken)
            where TLibgenObject : LibgenObject
            where TExportObject : ExportObject<TLibgenObject>
        {
            int fileSequence = 0;
            int fileRowCount = 0;
            int totalRowCount = 0;
            DateTime lastReportDateTime = DateTime.Now;
            TExportObject exportObject = null;
            TExportWriter exportWriter = null;
            string filePath = null;
            string firstFilePath = null;
            try
            {
                foreach (TLibgenObject libgenObject in libgenObjects)
                {
                    if (fileRowCount == 0)
                    {
                        fileSequence++;
                        if (fileSequence == 2)
                        {
                            string oldFilePath = GenerateFilePath(null, fileExtension);
                            string newFilePath = GenerateFilePath(1, fileExtension);
                            File.Move(oldFilePath, newFilePath);
                            firstFilePath = newFilePath;
                        }
                        filePath = GenerateFilePath(fileSequence == 1 ? (int?)null : fileSequence, fileExtension);
                        if (fileSequence == 1)
                        {
                            firstFilePath = filePath;
                        }
                        exportWriter = exportWriterFactory(filePath);
                        exportObject = exportObjectFactory(exportWriter);
                        foreach (string field in exportObject.FieldList)
                        {
                            exportWriter.WriteField(field);
                        }
                        exportWriter.EndRow();
                    }
                    exportObject.WriteObject(libgenObject);
                    exportWriter.EndRow();
                    totalRowCount++;
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return new ExportResult(totalRowCount, fileSequence, firstFilePath, isExportCancelled: true, isRowsPerFileLimitReached: false);
                    }
                    DateTime now = DateTime.Now;
                    if ((now - lastReportDateTime).TotalSeconds > SEARCH_PROGRESS_REPORT_INTERVAL)
                    {
                        progressHandler.Report(new ExportProgress(totalRowCount, fileSequence, isWriterDisposing: false));
                        lastReportDateTime = now;
                    }
                    fileRowCount++;
                    if (rowsPerFile.HasValue && fileRowCount == rowsPerFile.Value)
                    {
                        progressHandler.Report(new ExportProgress(totalRowCount, fileSequence, isWriterDisposing: true));
                        exportWriter.Dispose();
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return new ExportResult(totalRowCount, fileSequence, firstFilePath, isExportCancelled: true, isRowsPerFileLimitReached: false);
                        }
                        if (!splitIntoMultipleFiles)
                        {
                            return new ExportResult(totalRowCount, fileSequence, firstFilePath, isExportCancelled: false, isRowsPerFileLimitReached: true);
                        }
                        fileRowCount = 0;
                    }
                }
            }
            finally
            {
                if (exportWriter != null)
                {
                    progressHandler.Report(new ExportProgress(totalRowCount, fileSequence, isWriterDisposing: true));
                    exportWriter.Dispose();
                }
            }
            return new ExportResult(totalRowCount, fileSequence, firstFilePath, isExportCancelled: cancellationToken.IsCancellationRequested,
                isRowsPerFileLimitReached: false);
        }

        private string GenerateFilePath(int? fileSequence, string fileExtension)
        {
            StringBuilder filePathBuilder = new StringBuilder();
            filePathBuilder.Append(filePathTemplate);
            if (fileSequence.HasValue)
            {
                filePathBuilder.Append("_");
                filePathBuilder.Append(fileSequence.Value.ToString("N0"));
            }
            if (!String.IsNullOrWhiteSpace(fileExtension))
            {
                filePathBuilder.Append(".");
                filePathBuilder.Append(fileExtension);
            }
            return filePathBuilder.ToString();
        }
    }
}
