using System;

namespace LibgenDesktop.Models.Export
{
    internal abstract class ExportWriter : IDisposable
    {
        public abstract void WriteField(int value);
        public abstract void WriteField(long value);
        public abstract void WriteField(string value);
        public abstract void WriteField(DateTime value);
        public abstract void WriteField(DateTime? value);
        public abstract void EndRow();
        public abstract void Dispose();
    }
}
