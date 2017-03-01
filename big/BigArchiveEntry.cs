using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace sage.big
{
    public class BigArchiveEntry
    {
        private BigArchive m_archive;
        private readonly bool m_originallyInArchive;
        private long m_compressedSize;
        private long m_uncompressedSize;
        private string m_name;
        private BigStream m_stream;
        private long m_offset;

        public BigArchiveEntry(BigArchive archive,string name,long offset,long size)
        {
            m_archive = archive;
            m_name = name;
            m_offset = offset;
            m_uncompressedSize = size;
        }

        public BigArchive Archive => m_archive;

        public string FullName
        {
            get
            {
                return m_name;
            }
        }

        public long Length
        {
            get
            {
                return m_uncompressedSize;
            }
        }
    }

    public class BigStream : Stream
    {
        public override bool CanRead => throw new NotImplementedException();

        public override bool CanSeek => throw new NotImplementedException();

        public override bool CanWrite => throw new NotImplementedException();

        public override long Length => throw new NotImplementedException();

        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
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
    }
}
