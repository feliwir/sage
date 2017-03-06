using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace sage.vp6
{
    public class Demuxer
    {
        private readonly int AVP6_TAG = MAKE_TAG("AVP6");
        private readonly int MVhd_TAG = MAKE_TAG("MVhd");
        private readonly int MV0K_TAG = MAKE_TAG("MV0K");
        private readonly int MV0F_TAG = MAKE_TAG("MV0F");

        private static int MAKE_TAG(string tag)
        {
            char[] chars = tag.ToCharArray();
            byte[] bytes = Encoding.ASCII.GetBytes(chars);
            return BitConverter.ToInt32(bytes, 0);
        }

        private Stream m_stream;
        private BinaryReader m_reader;
        private List<Decoder> m_decoders;
        private bool m_initialized;
        private uint m_size;

        public Demuxer(Stream s)
        {
            m_stream = s;
            m_initialized = false;
            Init();
        }

        private void Init()
        {
            m_stream.Seek(0, SeekOrigin.Begin);
            if (!m_stream.CanRead)
                throw new ArgumentException("Stream must have Read permission!");

            if (!m_stream.CanSeek)
                throw new ArgumentException("Stream must have Seek permission!");

            m_reader = new BinaryReader(m_stream);
            if(!CheckProbe())
            {
                throw new InvalidDataException("Invalid FOURCC for VP6 Video!");
            }
            ProcessHeader();
        }

        private static int ReadReverseInt32(BinaryReader br)
        {
            byte[] array = br.ReadBytes(4);
            return BitConverter.ToInt32(array.Reverse().ToArray(), 0);
        }

        private static uint ReadReverseUInt32(BinaryReader br)
        {
            byte[] array = br.ReadBytes(4);
            return BitConverter.ToUInt32(array.Reverse().ToArray(), 0);
        }

        private void ProcessHeader()
        {
            uint headersize = m_reader.ReadUInt32();
            if (headersize < 8)
                throw new InvalidDataException("Invalid Headersize for VP6 Video!");

            bool check = true;
            while(check)
            {
                long startpos = m_stream.Position;
                int fourcc = m_reader.ReadInt32();
                int blocksize = m_reader.ReadInt32();
                if (fourcc==MVhd_TAG)
                {
                    ProcessVideoHeader();
                }
                else
                {
                    m_reader.BaseStream.Seek(-8, SeekOrigin.Current);
                    return;
                }
                m_stream.Seek(startpos +blocksize, SeekOrigin.Begin);
            }
            
        }

        private void ProcessVideoHeader()
        {
            char[] codec = m_reader.ReadChars(4);
            ushort width = m_reader.ReadUInt16();
            ushort height = m_reader.ReadUInt16();
            uint framecount = m_reader.ReadUInt32();
            uint largestFrame = m_reader.ReadUInt32();
            uint denominator = m_reader.ReadUInt32();
            uint numerator = m_reader.ReadUInt32();
            double fps = (double)denominator / numerator;
        }

        private bool CheckProbe()
        {
            int fourcc = m_reader.ReadInt32();
            if (fourcc == AVP6_TAG)
                return true;
            else
                return false;
        }
    }
}
