namespace sage.vp6
{
    internal class Macroblock
    {
        public static readonly sbyte[,] CandidatePredictor = new sbyte[12,2] {
            {  0, -1 },
            { -1,  0 },
            { -1, -1 },
            {  1, -1 },
            {  0, -2 },
            { -2,  0 },
            { -2, -1 },
            { -1, -2 },
            {  1, -2 },
            {  2, -1 },
            { -2, -2 },
            {  2, -2 },
        };

        private CodingMode m_type;
        private short m_vx;
        private short m_vy;

        public short Vx { get => m_vx; set => m_vx = value; }
        public short Vy { get => m_vy; set => m_vy = value; }
        public CodingMode Type { get => m_type; set => m_type = value; }
    }
}
