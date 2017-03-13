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

        private static readonly byte[,] DefaultVectorPdvModel = new byte[2, 7] {
            { 225, 146, 172, 147, 214,  39, 156 },
            { 204, 170, 119, 235, 140, 230, 228 },

        };

        private static readonly byte[,] DefaultRunvCoeffModel = new byte[2, 14] {
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
        private byte[,] m_coeffDccv = new byte[2, 11];
        private byte[,,,] m_coeffRact = new byte[2, 3, 6, 11];
        private byte[,,] m_coeffDcct = new byte[2, 36, 5];
        private byte[,,] m_defMbTypesStats;
        private byte[,,] m_mbType = new byte[3,10,10];

        public Model()
        {
            Default();
        }

        public byte[,] CoeffDccv { get => m_coeffDccv; set => m_coeffDccv = value; }
        public byte[] CoeffReorder { get => m_coeffReorder; set => m_coeffReorder = value; }
        public byte[] CoeffIndexToPos { get => m_coeffIndexToPos; set => m_coeffIndexToPos = value; }
        public byte[,] CoeffRunv { get => m_coeffRunv; set => m_coeffRunv = value; }
        public byte[,,,] CoeffRact { get => m_coeffRact; set => m_coeffRact = value; }
        public byte[,,] CoeffDcct { get => m_coeffDcct; set => m_coeffDcct = value; }
        public byte[,,] MacroblockType { get => m_mbType; set => m_mbType = value; }
        public byte[] VectorDct { get => m_vectorDct; set => m_vectorDct = value; }
        public byte[] VectorSig { get => m_vectorSig; set => m_vectorSig = value; }
        public byte[,] VectorFdv { get => m_vectorFdv; set => m_vectorFdv = value; }
        public byte[,] VectorPdv { get => m_vectorPdv; set => m_vectorPdv = value; }
        public byte[,] VectorPdi { get => m_vectorPdi; set => m_vectorPdi = value; }

        public void Default()
        {
            VectorDct[0] = 0xA2;
            VectorDct[1] = 0xA4;
            VectorSig[0] = 0x80;
            VectorSig[1] = 0x80;

            m_defMbTypesStats = DefaultMbTypesStats;
            VectorFdv = DefaultVectorFdvModel;
            VectorPdv = DefaultVectorPdvModel;
            CoeffReorder = DefaultCoeffReorder;
            CoeffRunv = DefaultRunvCoeffModel;

            InitializeCoeffOrderTable();

        }

        public void InitializeCoeffOrderTable()
        {
            byte i, pos, idx = 1;

            for (i = 0; i < 16; i++)
                for (pos = 1; pos < 64; pos++)
                    if (CoeffReorder[pos] == i)
                        CoeffIndexToPos[idx++] = pos;
        }

    }
}
