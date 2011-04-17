using System;
using System.IO;

namespace SpecLog.GitPlugin.Server
{
    public class NullStream : Stream
    {
        public override bool CanRead { get { return true; } }

        public override bool CanSeek { get { return true; } }

        public override bool CanWrite { get { return true; } }

        public override void Flush() { /* SKIP */ }

        private long length = 0;
        public override long Length { get { return length; } }

        private long position = 0;
        public override long Position
        {
            get { return position; }
            set { position = value; length = Math.Max(length, position); }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var size = Math.Min(count, length - position);
            for (int index = 0; index < size; ++index)
                buffer[offset + index] = 0;
            Position = position + size;
            return (int)size;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;
                case SeekOrigin.Current:
                    Position = position + offset;
                    break;
                case SeekOrigin.End:
                    Position = length - offset;
                    break;
                default:
                    break;
            }
            return position;
        }

        public override void SetLength(long value) { length = value; }

        public override void Write(byte[] buffer, int offset, int count) { Position = position + count; }
    }
}
