using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace sage.vp6
{
    public class Demuxer
    {
        private Stream m_stream;
        private BinaryReader m_reader;
        private List<Decoder> m_decoders;
        private bool m_initialized;

        public Demuxer(Stream s)
        {
            m_stream = s;
            m_initialized = false;
        }

        private void Init()
        {
            m_stream.Seek(0, SeekOrigin.Begin);
            if (!m_stream.CanRead)
                throw new ArgumentException("Stream must have Read permission!");

            if (!m_stream.CanSeek)
                throw new ArgumentException("Stream must have Seek permission!");

            m_reader = new BinaryReader(m_stream);
            ReadBlock();

        }

        private void ReadBlock()
        {
            char[] fourcc = m_reader.ReadChars(4);
            uint size = m_reader.ReadUInt32();
            if(fourcc.SequenceEqual(new char[] { 'M', 'V', 'h', 'd' }))
            {

            }
        }

        private void LoadHeader(uint size)
        {
            if(size!=24)
                throw new InvalidDataException("Invalid MVhd Header size!");
            

        }

    }
}
