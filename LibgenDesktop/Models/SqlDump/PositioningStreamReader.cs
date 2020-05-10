using System;
using System.IO;

namespace LibgenDesktop.Models.SqlDump
{
    internal class PositioningStream : Stream
    {
        private readonly Stream baseStream;
        private long position;

        public PositioningStream(Stream baseStream)
        {
            this.baseStream = baseStream;
            position = 0;
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotImplementedException();

        public override long Position
        {
            get => position;
            set => throw new NotImplementedException();
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int result = baseStream.Read(buffer, offset, count);
            if (result > 0)
            {
                position += result;
            }
            return result;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            baseStream?.Dispose();
        }
    }
}
