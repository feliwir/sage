using System;
using System.IO;
using System.Linq;
using System.Text;

namespace sage.vp6
{
    public class Demuxer
    {
        //VP6 WITH ALPHA
        private readonly int AVP6_TAG = MAKE_TAG("AVP6");
        //ALPHA MASK TAGS
        private readonly int AVhd_TAG = MAKE_TAG("AVhd");
        private readonly int AV0K_TAG = MAKE_TAG("AV0K");
        private readonly int AV0F_TAG = MAKE_TAG("AV0F");
        //VIDEO TAGS
        private readonly int MVhd_TAG = MAKE_TAG("MVhd");
        private readonly int MV0K_TAG = MAKE_TAG("MV0K");
        private readonly int MV0F_TAG = MAKE_TAG("MV0F");
        //KNOWN CODECS
        private readonly int vp60_TAG = MAKE_TAG("vp60");
        private readonly int vp61_TAG = MAKE_TAG("vp61");

        private static int MAKE_TAG(string tag)
        {
            char[] chars = tag.ToCharArray();
            byte[] bytes = Encoding.ASCII.GetBytes(chars);
            return BitConverter.ToInt32(bytes, 0);
        }

        private Stream m_stream;
        private BinaryReader m_reader;
        private Context m_video;
        private Context m_alpha;
        private int m_fourcc;

        public Context Video { get => m_video; set => m_video = value; }
        public Context Alpha { get => m_alpha; set => m_alpha = value; }

        public Demuxer(Stream s)
        {
            m_stream = s;
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
            CheckProbe();
            
            ProcessHeader();
            //Read first packets
            ReadPacket();
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

            if (m_fourcc==AVP6_TAG)
            {
                while (m_alpha==null || m_video == null)
                {
                    long startpos = m_stream.Position;
                    int fourcc = m_reader.ReadInt32();
                    int blocksize = m_reader.ReadInt32();
                    if (fourcc == MVhd_TAG)
                    {
                        ProcessVideoHeader(StreamType.VIDEO);
                    }
                    else if (fourcc == AVhd_TAG)
                    {
                        ProcessVideoHeader(StreamType.ALPHA);
                    }
                    else
                    {
                        m_reader.BaseStream.Seek(-8, SeekOrigin.Current);
                        break;
                    }
                    m_stream.Seek(startpos + blocksize, SeekOrigin.Begin);
                }
            }
            else if(m_fourcc == MVhd_TAG)
            {
                ProcessVideoHeader(StreamType.VIDEO);
            }                
        }

        private void ReadPacket()
        {
            bool read = !(m_reader.BaseStream.Position == m_reader.BaseStream.Length);
            while(read)
            {
                int chunk_type = m_reader.ReadInt32();
                int chunk_size = m_reader.ReadInt32();

                if(chunk_size<8)
                    throw new InvalidDataException("A packet must be atleast 8 bytes long!");

                //Pass packet to the video stream
                if (chunk_type == MV0K_TAG || chunk_type == MV0F_TAG)
                {
                    Video.ProcessPacket(m_reader, chunk_size);
                }
                //Pass packet to the alpha stream
                else if(chunk_type == AV0K_TAG || chunk_type == AV0F_TAG)
                {
                    m_alpha.ProcessPacket(m_reader, chunk_size);
                }
                else
                {
                    byte[] buf = BitConverter.GetBytes(chunk_type);
                    throw new InvalidDataException("Unknown Chunktype: " + System.Text.Encoding.ASCII.GetString(buf));
                }

                if (m_reader.BaseStream.Position == m_reader.BaseStream.Length)
                    read = false;
            }
        }

        private void ProcessVideoHeader(StreamType type)
        {
            CheckCodec();
            
            ushort width = m_reader.ReadUInt16();
            ushort height = m_reader.ReadUInt16();
            uint framecount = m_reader.ReadUInt32();
            uint largestFrame = m_reader.ReadUInt32();
            uint denominator = m_reader.ReadUInt32();
            uint numerator = m_reader.ReadUInt32();
            double fps = (double)denominator / numerator;

            switch (type)
            {
                case StreamType.VIDEO:
                    Video = new Context(width, height, denominator, numerator, framecount,StreamType.VIDEO);
                    break;
                case StreamType.ALPHA:
                    m_alpha = new Context(width, height, denominator, numerator, framecount, StreamType.ALPHA);
                    break;
            }
        }

        private void CheckProbe()
        {
            int fourcc = m_reader.ReadInt32();
            if (!(fourcc == AVP6_TAG||
                  fourcc == MVhd_TAG))

            {
                byte[] buf = BitConverter.GetBytes(fourcc);
                throw new InvalidDataException("Unknown FOURCC: " + System.Text.Encoding.ASCII.GetString(buf));
            }

            m_fourcc = fourcc;
        }

        private void CheckCodec()
        {
            int fourcc = m_reader.ReadInt32();
            if (!(fourcc == vp60_TAG||
                  fourcc == vp61_TAG))
            {
                byte[] buf = BitConverter.GetBytes(fourcc);
                throw new InvalidDataException("Unknown CODEC: " + System.Text.Encoding.ASCII.GetString(buf));
            }
        }
    }
}
