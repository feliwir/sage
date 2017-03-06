using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace sage.vp6
{
    /// <summary>
    /// Vp6 supports INTRA and INTER Frames
    /// </summary>
    public enum FrameType
    {
        /// <summary>
        /// Can be decoded without a previous frame. Also called Keyframe
        /// </summary>
        INTRA = 0,
        /// <summary>
        /// Requires a reference frame
        /// </summary>
        INTER = 1
    }


    class Frame
    {
        private FrameType m_type;
        private int m_quantizer;
        private Format m_format;
        private Profile m_profile;
        private bool m_seperateCoeff;
        private short m_coeffOffset;
        private byte m_rows;
        private byte m_cols;
        //Read the FrameHeader
        public Frame(byte[] buf)
        {
            int index = 0;
            m_type = (FrameType)(buf[index] & 0x80);
            m_quantizer = (buf[index] >> 1) & 0x3F;
            m_seperateCoeff = Convert.ToBoolean(buf[index] & 0x01);
            ++index;

            switch (m_type)
            {
                case FrameType.INTRA:
                    m_format = (Format)(buf[index] >> 3);
                    m_profile = (Profile)(buf[index] & 0x06);
                    ++index;

                    if(m_seperateCoeff || m_profile == Profile.SIMPLE)
                    {
                        m_coeffOffset = BitConverter.ToInt16(buf, index);
                        index += 2;
                    }

                    m_rows = buf[index++];
                    m_cols = buf[index++];

                    if(m_rows==0||m_cols==0)
                    {
                        throw new InvalidDataException("Invalid size!");
                    }

                    break;
                case FrameType.INTER:
                    break;
            }
        }


        public FrameType Type { get => m_type; set => m_type = value; }
    }
}
