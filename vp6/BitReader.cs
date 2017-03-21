using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace sage.vp6
{
    class BitReader
    {
        private BitArray m_bitArray;
        private int m_bitIndex;

        public BitReader(byte[] buffer,int index)
        {
            m_bitArray = new BitArray(buffer);
            m_bitIndex = (index) << 3;
        }

        public int BitsLeft()
        {
            return m_bitArray.Length - m_bitIndex;
        }

        public int GetBits(int n)
        {
            int result = 0;

            for(int i=0;i<n;++i,++m_bitIndex)
            {
                result |= (Convert.ToInt32(m_bitArray[m_bitIndex]) << i);
            }

            return result;
        }

        public int GetBit()
        {
            return (Convert.ToInt32(m_bitArray[m_bitIndex++]));
        }
    }
}
