namespace sage.vp6
{
    internal class RangeDecoder
    {
        internal byte[] NormShift = new byte[256] {
            8,7,6,6,5,5,5,5,4,4,4,4,4,4,4,4,
            3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,
            2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,
            2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,
            1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
            1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
            1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
            1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
        };

        int m_startpos;
        int m_index;
        int m_bit;
        byte[] m_buffer;
        byte m_high;
        sbyte m_bits;
        ulong m_codeword;

        public RangeDecoder(byte[] buffer,int startpos)
        {
            m_index = 1;
            m_startpos = startpos;
            m_buffer = buffer;
            m_codeword = (ulong)(buffer[startpos+m_index++] << 8);
            m_codeword |= (ulong)(buffer[startpos + m_index++]);
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
