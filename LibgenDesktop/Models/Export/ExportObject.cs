using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using LibgenDesktop.Models.Entities;

namespace LibgenDesktop.Models.Export
{
    internal abstract class ExportObject<T> where T : LibgenObject
    {
        public ExportObject(ExportWriter exportWriter)
        {
            ExportWriter = exportWriter;
        }

        public abstract IEnumerable<string> FieldList { get; }

        protected ExportWriter ExportWriter { get; }

        public abstract void WriteObject(T libgenObject);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void WriteField(int value)
        {
            ExportWriter.WriteField(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void WriteField(long value)
        {
            ExportWriter.WriteField(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void WriteField(string value)
        {
            ExportWriter.WriteField(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void WriteField(DateTime value)
        {
            ExportWriter.WriteField(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void WriteField(DateTime? value)
        {
            ExportWriter.WriteField(value);
        }
    }
}
