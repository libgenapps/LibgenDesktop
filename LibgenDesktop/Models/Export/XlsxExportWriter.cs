using System;
using System.IO;
using System.Runtime.CompilerServices;
using OfficeOpenXml;

namespace LibgenDesktop.Models.Export
{
    internal class XlsxExportWriter : ExportWriter
    {
        private readonly string filePath;
        private readonly ExcelPackage excelPackage;
        private readonly ExcelWorksheet worksheet;
        private int currentRowIndex;
        private int currentColumnIndex;
        private bool disposed;

        public XlsxExportWriter(string filePath)
        {
            this.filePath = filePath;
            excelPackage = new ExcelPackage();
            worksheet = excelPackage.Workbook.Worksheets.Add("Libgen-export");
            currentRowIndex = 1;
            currentColumnIndex = 1;
            disposed = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteField(int value)
        {
            WriteObject(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteField(long value)
        {
            WriteObject(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteField(string value)
        {
            WriteObject(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteField(DateTime value)
        {
            worksheet.Cells[currentRowIndex, currentColumnIndex].Style.Numberformat.Format = "yyyy-MM-dd hh:mm:ss";
            WriteObject(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteField(DateTime? value)
        {
            if (value.HasValue)
            {
                WriteField(value.Value);
            }
            else
            {
                currentColumnIndex++;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void EndRow()
        {
            currentRowIndex++;
            currentColumnIndex = 1;
        }

        public override void Dispose()
        {
            if (!disposed)
            {
                excelPackage.SaveAs(new FileInfo(filePath));
                worksheet.Dispose();
                excelPackage.Dispose();
                disposed = true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteObject(object value)
        {
            worksheet.SetValue(currentRowIndex, currentColumnIndex, value);
            currentColumnIndex++;
        }
    }
}
