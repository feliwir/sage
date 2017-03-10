using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace sage.refpack
{
    public class Compressor
    {
        private BinaryReader br;
        private MemoryStream ms;
        private BinaryWriter bw;

        public Compressor(Stream stream)
        {
            br = new BinaryReader(stream);
        }

        public MemoryStream Compress()
        {
            ms = new MemoryStream();

            WriteHeader();
            CompressionLoop();

            return ms;
        }

        void WriteHeader()
        {
            long length = br.BaseStream.Length;
            bool is_long_src = (length > 0xFFFFFF);

            /* 2 bytes header */
            byte[] hdr = { (byte)(is_long_src ? 0x90 : 0x10), 0xFB };
            bw.Write(hdr, 0, 2);

            /* 3 or 4 bytes big-endian source size */
            if (is_long_src)
                bw.Write((UInt32)length);
            else
            {
                bw.Write((byte)length >> 16);
                bw.Write((byte)length >> 8);
                bw.Write((byte)length);
            }
        }

        void CompressionLoop()
        {

            /* read ahead some data into buffer */
            //ReadAhead(ei, 2 * 0x8000);



            //while (m_src_pos < m_src_end_pos)
            //{
            //    CompressionOneStep();

            //    if (m_src_pos + cNextIndexLength >= m_buf_end_pos && m_buf_end_pos < m_src_end_pos)
            //    {
            //        ReadAhead(ei, cNextIndexLength);
            //    }
            //}
        }
    }
}
