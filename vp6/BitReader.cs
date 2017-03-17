using System;
using System.Collections.Generic;
using System.Text;

namespace sage.vp6
{
    class BitReader
    {
        private byte[] m_buffer;
        private int m_numBits;
        private int m_index;
        private int m_bitSize;

        public BitReader(byte[] buffer,int index)
        {
            m_bitSize = (buffer.Length - index) << 3;
        }
    }
}
