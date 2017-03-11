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
        private Motionvector m_mv;

        public CodingMode Type { get => m_type; set => m_type = value; }
        internal Motionvector Mv { get => m_mv; set => m_mv = value; }
    }
}
