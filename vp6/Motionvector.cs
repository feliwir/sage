using System;
using System.Collections.Generic;
using System.Text;

namespace sage.vp6
{
    struct Motionvector
    {
        private int m_x;
        private int m_y;

        public Motionvector(int x,int y)
        {
            m_x = x;
            m_y = y;
        }

        public int X { get => m_x; set => m_x = value; }
        public int Y { get => m_y; set => m_y = value; }
    }
}
