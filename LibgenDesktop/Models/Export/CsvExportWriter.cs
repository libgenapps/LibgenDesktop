using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace LibgenDesktop.Models.Export
{
    internal class CsvExportWriter : ExportWriter
    {
        private const char ZERO_WIDTH_SPACE = '\u200b';

        private readonly StreamWriter streamWriter;
        private readonly char separator;
        private bool firstField;
        private bool disposed;

        public CsvExportWriter(string filePath, char separator)
        {
            streamWriter = new StreamWriter(filePath, false, Encoding.UTF8);
            this.separator = separator;
            firstField = true;
            disposed = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteField(int value)
        {
            WriteSeparatorIfNecessary();
            streamWriter.Write(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteField(long value)
        {
            WriteSeparatorIfNecessary();
            streamWriter.Write(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteField(string value)
        {
            WriteSeparatorIfNecessary();
            value = value.Replace("\"", "\"\"");
            if (separator == '\t')
            {
                value = value.Replace("\t", String.Empty);
            }
            value = String.Concat("\"", ZERO_WIDTH_SPACE, value, "\"");
            streamWriter.Write(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteField(DateTime value)
        {
            WriteSeparatorIfNecessary();
            streamWriter.Write(value.ToString("yyyy-MM-dd HH:mm:ss"));
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
                WriteSeparatorIfNecessary();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void EndRow()
        {
            streamWriter.WriteLine();
            firstField = true;
        }

        public override void Dispose()
        {
            if (!disposed)
            {
                streamWriter.Dispose();
                disposed = true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteSeparatorIfNecessary()
        {
            if (firstField)
            {
                firstField = false;
            }
            else
            {
                streamWriter.Write(separator);
            }
        }
    }
}
