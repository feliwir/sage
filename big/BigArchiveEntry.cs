using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace sage.big
{
    /// <summary>
    /// This class represents a single entry inside an archive
    /// </summary>
    public class BigArchiveEntry
    {
        private BigArchive m_archive;
        private readonly bool m_originallyInArchive;
        private long m_compressedSize;
        private long m_uncompressedSize;
        private string m_name;
        private long m_offset;

        public BigArchiveEntry(BigArchive archive, string name, long offset, long size)
        {
            m_archive = archive;
            m_name = name;
            m_offset = offset;
            m_uncompressedSize = size;
        }

        public BigArchive Archive => m_archive;

        /// <summary>
        /// The complete name (with directories) of an entry
        /// </summary>
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

        public Stream Open()
        {
            switch (m_archive.Mode)
            {
                case BigArchiveMode.Read:
                    return OpenInReadMode();
                case BigArchiveMode.Create:
                    return OpenInWriteMode();
                case BigArchiveMode.Update:
                default:
                    return OpenInUpdateMode();
            }
        }

        public Stream OpenInUpdateMode()
        {
            return new BigStream(this, true, m_offset);
        }

        public Stream OpenInReadMode()
        {
            return new BigStream(this, false, m_offset);
        }

        public Stream OpenInWriteMode()
        {
            return new BigStream(this, true, m_offset);
        }
    }

    public class BigStream : Stream
    {
        private BigArchive m_archive;
        private BigArchiveEntry m_entry;
        private bool m_writable;
        private long m_offset;
        private long m_pos = 0;

        public BigStream(BigArchiveEntry entry, bool writable, long offset)
        {
            m_entry = entry;
            m_archive = entry.Archive;
            m_writable = writable;
            m_offset = offset;
        }

        public override bool CanRead => m_archive.ArchiveStream.CanRead;

        public override bool CanSeek => m_archive.ArchiveStream.CanSeek;

        public override bool CanWrite => m_writable;

        public override long Length => m_entry.Length;

        public override long Position
        {
            get { return m_pos; }
            set { m_pos = value; }
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            m_archive.ArchiveStream.Seek(m_offset + m_pos, SeekOrigin.Begin);
            int result = m_archive.ArchiveStream.Read(buffer, offset, count);
            m_pos += result;
            return result;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch(origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;
                case SeekOrigin.Current:
                    Position += offset;
                    break;
                case SeekOrigin.End:
                    Position = Length + offset;
                    break;
            }
            return Position;
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
