using System;
using System.Collections.Generic;
using System.Text;

namespace sage.vp6
{
    internal class BitReader
    {
        int m_startpos;
        int m_index;
        int m_bit;
        byte[] m_buffer;

        public BitReader(byte[] buffer,int startpos)
        {
            m_startpos = startpos;
            m_buffer = buffer;
        }

        public int ReadBits(int bits)
        {
            int value = 0;

            while(bits>0)
            {
                value = (value << 1) | ReadBit();
                --bits;
            }

            return value;
        }

        public int ReadBit()
        {
            byte cbyte = m_buffer[m_startpos + m_index];
            m_bit++;
            int value = (cbyte << m_bit) & 0x01;
            if(m_bit==8)
            {
                m_bit = 0;
                m_index++;
            }

            return value;
        }
    }
}
