using System;
using System.Collections.Generic;
using System.Text;

namespace sage.vp6
{
    class Model
    {
        private static readonly byte[,,] DefaultMbTypesStats = new byte[3, 10, 2] {
            { {  69, 42 }, { 1, 2 }, { 1,  7 }, { 44, 42 }, { 6, 22 },
              {   1,  3 }, { 0, 2 }, { 1,  5 }, {  0,  1 }, { 0,  0 }, },
            { { 229,  8 }, { 1, 1 }, { 0,  8 }, {  0,  0 }, { 0,  0 },
              {   1,  2 }, { 0, 1 }, { 0,  0 }, {  1,  1 }, { 0,  0 }, },
            { { 122, 35 }, { 1, 1 }, { 1,  6 }, { 46, 34 }, { 0,  0 },
              {   1,  2 }, { 0, 1 }, { 0,  1 }, {  1,  1 }, { 0,  0 }, },
        };

        private static readonly byte[,] DefaultVectorFdvModel = new byte[2, 8] {
            { 247, 210, 135, 68, 138, 220, 239, 246 },
            { 244, 184, 201, 44, 173, 221, 239, 253 },
        };

        private static readonly byte[,] DefaultVectorPdvModel = new byte[2,7] {
            { 225, 146, 172, 147, 214,  39, 156 },
            { 204, 170, 119, 235, 140, 230, 228 },

        };

        private static readonly byte[,] DefaultRunvCoeffModel = new byte[2,14] {
            { 198, 197, 196, 146, 198, 204, 169, 142, 130, 136, 149, 149, 191, 249 },
            { 135, 201, 181, 154,  98, 117, 132, 126, 146, 169, 184, 240, 246, 254 },
        };

        private static readonly byte[] DefaultCoeffReorder = new byte[] {
             0,  0,  1,  1,  1,  2,  2,  2,
             2,  2,  2,  3,  3,  4,  4,  4,
             5,  5,  5,  5,  6,  6,  7,  7,
             7,  7,  7,  8,  8,  9,  9,  9,
             9,  9,  9, 10, 10, 11, 11, 11,
            11, 11, 11, 12, 12, 12, 12, 12,
            12, 13, 13, 13, 13, 13, 14, 14,
            14, 14, 15, 15, 15, 15, 15, 15,
        };

        private byte[] m_coeffReorder = new byte[64];
        private byte[] m_coeffIndexToPos = new byte[64];
        private byte[] m_vectorSig = new byte[2];
        private byte[] m_vectorDct = new byte[2];
        private byte[,] m_vectorPdi = new byte[2, 2];
        private byte[,] m_vectorPdv = new byte[2, 7];
        private byte[,] m_vectorFdv = new byte[2, 8];
        private byte[,] m_coeffRunv = new byte[2, 14];
        private byte[,,] m_defMbTypesStats;

        public Model()
        {
            Default();
        }

        public void Default()
        {
            m_vectorDct[0] = 0xA2;
            m_vectorDct[1] = 0xA4;
            m_vectorSig[0] = 0x80;
            m_vectorSig[1] = 0x80;

            m_defMbTypesStats = DefaultMbTypesStats;
            m_vectorFdv = DefaultVectorFdvModel;
            m_vectorPdv = DefaultVectorPdvModel;
            m_coeffReorder = DefaultCoeffReorder;
            m_coeffRunv = DefaultRunvCoeffModel;

            CoeffOrderTable();

        }

        private void CoeffOrderTable()
        {
            byte i, pos, idx = 1;

            for (i = 0; i < 16; i++)
                for (pos = 1; pos < 64; pos++)
                    if (m_coeffReorder[pos] == i)
                        m_coeffIndexToPos[idx++] = pos;
        }

    }
}
