using System;
using System.Collections.Generic;
using System.Text;

namespace sage.vp6
{
    struct Reference
    {
        private byte m_notNullDc;
        private int m_refFrame;
        private short m_dcCoeff;

        public byte NotNullDc { get => m_notNullDc; set => m_notNullDc = value; }
        public int RefFrame { get => m_refFrame; set => m_refFrame = value; }
        public short DcCoeff { get => m_dcCoeff; set => m_dcCoeff = value; }
    }
}
