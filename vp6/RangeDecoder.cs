﻿using System;

namespace sage.vp6
{
    internal class RangeDecoder
    {
        int m_startpos;
        int m_index;
        byte[] m_buffer;
        int m_high;
        int m_bits;
        uint m_codeword;

        public RangeDecoder(byte[] buffer, int startpos)
        {
            m_index = 0;
            m_high = 255;
            m_bits = -16;
            m_startpos = startpos;
            m_buffer = buffer;
            m_codeword = (uint)(m_buffer[m_startpos + m_index++] << 16);
            m_codeword |= (uint)(m_buffer[m_startpos + m_index++] << 8);
            m_codeword |= (uint)(m_buffer[m_startpos + m_index++]);
        }

        public int ReadBitsNn(int bits)
        {
            int v = ReadBits(bits) << 1;
            return v + Convert.ToInt32(!Convert.ToBoolean(v));
        }

        public int ReadBits(int bits)
        {
            int value = 0;

            while (bits > 0)
            {
                value = (value << 1) | ReadBit();
                --bits;
            }

            return value;
        }

        public int ReadBit()
        {
            uint codeword = Renormalize();
            int low = (m_high + 1) >> 1;
            uint low_shift = (uint)low << 16;

            bool bit = codeword >= low_shift;
            if (bit)
            {
                m_high -= (byte)low;
                codeword -= low_shift;
            }
            else
            {
                m_high = (byte)low;
            }

            m_codeword = codeword;
            return Convert.ToInt32(bit);
        }

        /// <summary>
        /// This function is used if there occurs branching when the bit is true
        /// </summary>
        /// <param name="prob">the probability</param>
        /// <returns></returns>
        public int GetBitProbabilityBranch(int prob)
        {
            uint codeword = Renormalize();
            uint low = (uint)(1 + (((m_high - 1) * prob) >> 8));
            uint low_shift = low << 16;

            if (codeword >= low_shift)
            {
                m_high -= (int)low;
                m_codeword = codeword - low_shift;
                return 1;
            }

            m_codeword = codeword;
            m_high = (int)low;
            return 0;
        }

        /// <summary>
        /// This function gets the probability for the bitstream
        /// </summary>
        /// <param name="prob"></param>
        /// <returns></returns>
        public int GetBitProbability(int prob)
        {
            uint codeword = Renormalize();
            uint low = (uint)(1 + (((m_high - 1) * prob) >> 8));
            uint low_shift = low << 16;
            int bit = Convert.ToInt32(codeword >= low_shift);

            m_high = (int)(bit>0 ? m_high - low : low);
            m_codeword = bit>0 ? codeword - low_shift : codeword;

            return bit;
        }

        private uint Renormalize()
        {
            int shift = Data.NormShift[m_high];
            int bits = m_bits;
            uint codeword = m_codeword;
            uint tmp = 0;
            m_high <<= shift;
            codeword <<= shift;
            bits += shift;

            if (bits >= 0 && (m_startpos + m_index)+1 < m_buffer.Length)
            {
                tmp |= (uint)(m_buffer[m_startpos + m_index++] << 8);
                tmp |= (uint)(m_buffer[m_startpos + m_index++]);
                codeword |= tmp << bits;
                bits -= 16;
            }
            m_bits = bits;
            return codeword;
        }

        public int GetTree(Tree[] tree,byte[] probs)
        {
            int index = 0;
            while(tree[index].Value>0)
            {
                if(GetBitProbabilityBranch(probs[tree[index].ProbIdx])>0)
                {
                    index += tree[index].Value;
                }
                else
                {
                    index++;
                }
            }

            return -tree[index].Value;
        }
    }

    internal struct Tree
    {
        sbyte m_value;
        sbyte m_probIdx;

        public Tree(sbyte value,sbyte probIdx)
        {
            m_value = value;
            m_probIdx = probIdx;
        }

        public Tree(sbyte value) :this(value,0)
        {          
        }

        public Tree(CodingMode value,bool neg) : this((sbyte)(neg ?-(sbyte)value:(sbyte)value), 0)
        {
        }

        public sbyte Value { get => m_value; set => m_value = value; }
        public sbyte ProbIdx { get => m_probIdx; set => m_probIdx = value; }
    }
}
