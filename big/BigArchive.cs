using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace sage.big
{
    public enum BigArchiveVersion
    {
        BIG4,
        BIGF
    }

    public class BigArchive : IDisposable
    {
        private Stream m_stream;
        private BinaryReader m_reader;
        private List<BigArchiveEntry> m_entries;
        private Dictionary<string, BigArchiveEntry> m_entriesDictionary;
        private ReadOnlyCollection<BigArchiveEntry> m_entriesCollection;
        private BigArchiveMode m_mode;
        private BigArchiveVersion m_version;
        private bool m_leaveOpen;
        private int m_size;
        private int m_first;
        private int m_numEntries;
        
        public BigArchive(Stream stream) : this(stream, BigArchiveMode.Read, leaveOpen: false)
        {      
        }

        public BigArchive(Stream stream, BigArchiveMode mode) : this(stream, mode, leaveOpen: false)
        {
        }

        public BigArchive(Stream stream, BigArchiveMode mode, bool leaveOpen)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            Init(stream, mode, leaveOpen);
        }

        private void Init(Stream stream, BigArchiveMode mode, bool leaveOpen)
        {
            switch (mode)
            {
                case BigArchiveMode.Create:
                    if (!stream.CanWrite)
                        throw new ArgumentException("Stream must have Write permission!");
                    break;
                case BigArchiveMode.Read:
                    if (!stream.CanRead)
                        throw new ArgumentException("Stream must have Read permission!");
                    if (!stream.CanSeek)
                        throw new ArgumentException("Stream must have Seek permission!");
                    break;
                case BigArchiveMode.Update:
                    if (!stream.CanRead || !stream.CanWrite || !stream.CanSeek)
                        throw new ArgumentException("Stream must have Write,Read and Seek permission!");
                    break;
            }

            m_mode = mode;
            m_stream = stream;
            if (mode == BigArchiveMode.Create)
                m_reader = null;
            else
                m_reader = new BinaryReader(m_stream);

            m_entries = new List<BigArchiveEntry>();
            m_entriesCollection = new ReadOnlyCollection<BigArchiveEntry>(m_entries);
            m_entriesDictionary = new Dictionary<string, BigArchiveEntry>();
            m_leaveOpen = leaveOpen;

            switch (mode)
            {
                case BigArchiveMode.Read:
                case BigArchiveMode.Update:
                    Read();
                    break;
            }
        }

        private int ReadReverseInt32(BinaryReader br)
        {
            byte[] array = br.ReadBytes(4);
            return BitConverter.ToInt32(array.Reverse().ToArray(), 0);
        }

        private string ReadNullterminatedString(BinaryReader br)
        {
            string result = "";
            char c;
            while ((c = br.ReadChar()) != '\0')
                result += c;

            return result;
        }

        private void Read()
        {
            //start parsing the big
            char[] magic = m_reader.ReadChars(4);
            if (magic.SequenceEqual(new char[] { 'B', 'I', 'G', '4' }))
                m_version = BigArchiveVersion.BIG4;
            else if (magic.SequenceEqual(new char[] { 'B', 'I', 'G', 'F' }))
                m_version = BigArchiveVersion.BIGF;
            else
                throw new InvalidDataException("Not a known BIG format!");

            m_size = m_reader.ReadInt32();
            m_numEntries = ReadReverseInt32(m_reader);
            m_first = ReadReverseInt32(m_reader);

            foreach (var i in Enumerable.Range(0, m_numEntries))
            {
                // do something
                int offset = ReadReverseInt32(m_reader);
                int size = ReadReverseInt32(m_reader);
                string name = ReadNullterminatedString(m_reader);
                AddEntry(new BigArchiveEntry(this, name, offset, size));
            }

        }

        private void AddEntry(BigArchiveEntry entry)
        {
            m_entries.Add(entry);

            string entryName = entry.FullName;
            if (!m_entriesDictionary.ContainsKey(entryName))
            {
                m_entriesDictionary.Add(entryName, entry);
            }
        }

        public BigArchiveEntry CreateEntry()
        {
            return null;
        }

        public void Dispose()
        {
            if (!m_leaveOpen)
            {
                m_stream.Dispose();
                m_reader?.Dispose();
            }
        }

        public ReadOnlyCollection<BigArchiveEntry> Entries
        {
            get
            {
                return m_entriesCollection;
            }
        }

        public BigArchiveEntry GetEntry(string entryName)
        {
            if (entryName == null)
                throw new ArgumentNullException(nameof(entryName));

            if (m_mode == BigArchiveMode.Create)
                throw new NotSupportedException("Can't get entry in create mode!");

            BigArchiveEntry result;
            m_entriesDictionary.TryGetValue(entryName, out result);
            return result;
        }

        public BigArchiveMode Mode
        {
            get
            {
                return m_mode;
            }
        }

        public BigArchiveVersion Version
        {
            get
            {
                return m_version;
            }
        }
    }
}
