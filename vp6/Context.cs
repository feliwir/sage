using System;
using System.Collections.Generic;
using System.IO;

namespace sage.vp6
{
    //Decoder Context for VP6
    public class Context
    {
        //STREAM INFORMATION
        private uint m_width;
        private uint m_height;
        private uint m_mbWidth;
        private uint m_mbHeight;
        private uint m_denominator;
        private uint m_numerator;
        private uint m_framecount;
        private StreamType m_type;
        private int m_flip;

        //REQUIRED FOR DECODING 
        private Frame[] m_frames;
        private CodingMode m_macroblockType;
        private Model m_model;
        private RangeDecoder m_rangeDec;
        private RangeDecoder m_coeffDec;
        private Profile m_profile;
        private bool m_useLoopFiltering;
        private bool m_loopFilterSelector;
        private Format m_format;
        private Macroblock[] m_macroblocks;
        private Reference[] m_aboveBlocks;
        private Reference[] m_leftBlocks;
        private int[] m_aboveBlocksIdx;
        private int m_vectorCandidatePos;
        private Motionvector[] m_mvs;
        private Motionvector[] m_vectorCandidate;
        private short[,] m_blockCoeff;
        private IDCT m_idct;
        private int[] m_blockOffset;
        private int m_frbi;
        private int m_srbi;

        //REQUIRED for prediction
        private short[,] m_prevDc;
        //Information regarding the frames
        private uint m_yStride;
        private uint m_uvStride;
        private uint m_ySize;
        private uint m_uvSize;

        public Context(uint width, uint height, uint denominator, uint numerator,
            uint framecount, bool flip,StreamType type)
        {
            Width = width;
            Height = height;
            Denominator = denominator;
            Numerator = numerator;
            Framecount = framecount;
            Type = type;
            Mvs = new Motionvector[6];
            Frames = new Frame[3];
            LeftBlocks = new Reference[4];
            Model = new Model();
            PrevDc = new short[3, 3];
            AboveBlocksIdx = new int[6];
            VectorCandidate = new Motionvector[2];
            BlockCoeff = new short[6, 64];
            BlockOffset = new int[6];
            Idct = new IDCT();

            if (flip)
            {
                Flip = -1;
                Frbi = 2;
                Srbi = 0;
            }
            else
            {
                Flip = 1;
                Frbi = 0;
                Srbi = 2;
            }
        }

        /// <summary>
        /// Tells if this decoder is requiring a new packet
        /// </summary>
        /// <returns>Is a packet required</returns>
        public bool RequirePacket()
        {
            return m_frames[FrameSelect.CURRENT]==null;
        }

        public void ProcessPacket(BinaryReader br, int packet_size)
        {
            Frames[FrameSelect.PREVIOUS] = Frames[FrameSelect.CURRENT];
            //Substract the first 8 bytes that have already been read
            byte[] buffer = br.ReadBytes(packet_size - 8);
            
            //One packet is always exactly one frame
            Frame frame = m_frames[FrameSelect.CURRENT] = new Frame(buffer, this);

            //Decode this frame
            frame.Decode(this);

            //The golden frame is either the last INTRA frame or an INTER frame that was marked as golden frame
            if (frame.IsGolden||frame.Type==FrameType.INTRA)
            {
                Frames[FrameSelect.GOLDEN] = frame;
            }

            Frames[FrameSelect.CURRENT] = frame;
        }

        public void ParseCoefficientsHuffman(int dequant_ac)
        {

        }

        public void ParseCoefficients(int dequant_ac)
        {
            int ctx,coeff, coeff_index,idx;
            int pt = 0,sign,cg;
            byte[] model1, model2,model3;

            for (int b=0;b<6;++b)
            {
                //codetyps
                int ct = 1;
                int run = 1;

                if (b > 3)
                    pt = 1;

                ctx = LeftBlocks[Data.B6To4[b]].NotNullDc + AboveBlocks[AboveBlocksIdx[b]].NotNullDc;
                model1 = Util.GetSlice<byte>(Model.CoeffDccv, pt);
                model2 = Util.GetSlice<byte>(Model.CoeffDcct, pt, ctx);

                coeff_index = 0;
                for(;;)
                {
                    if((coeff_index>1 && ct==0)|| CoeffDec.GetBitProbabilityBranch(model2[0])>0)
                    {
                        //Parse a coefficient
                        if(CoeffDec.GetBitProbabilityBranch(model2[2])>0)
                        {
                            if (CoeffDec.GetBitProbabilityBranch(model2[3]) > 0)
                            {
                                idx = CoeffDec.GetTree(Data.PcTree, model1);
                                coeff = Data.CoeffBias[idx + 5];
                                for (int i = Data.CoeffBitLength[idx]; i >= 0; --i)
                                    coeff += CoeffDec.GetBitProbability(Data.CoeffParseTable[idx, i]) << i;
                            }
                            else
                            {
                                if (CoeffDec.GetBitProbabilityBranch(model2[4]) > 0)
                                    coeff = 3 + CoeffDec.GetBitProbability(model1[5]);
                                else
                                    coeff = 2;
                            }

                            ct = 2;
                        }
                        else
                        {
                            ct = 1;
                            coeff = 1;
                        }

                        sign = CoeffDec.ReadBit();
                        coeff = (coeff ^ -sign) + sign;
                        if (coeff_index>0)
                            coeff *= dequant_ac;

                        idx = Model.CoeffIndexToPos[coeff_index];
                        BlockCoeff[b, Data.Scantable[idx]] = (short)coeff;
                        run = 1;
                    }
                    //Parse a run
                    else
                    {
                        ct = 0;
                        if(coeff_index>0)
                        {
                            if (CoeffDec.GetBitProbabilityBranch(model2[1]) <= 0)
                                break;

                            model3 = Util.GetSlice(Model.CoeffRunv,Convert.ToInt32(coeff_index>=6));
                            run = CoeffDec.GetTree(Data.PcrTree, model3);
                            if(run<=0)
                            {
                                run = 9;
                                for (int i=0;i<6;++i)
                                {
                                    run += CoeffDec.GetBitProbability(model3[i + 8])<<i;
                                }
                            }
                        }
                    }

                    coeff_index += run;
                    if (coeff_index >= 64)
                        break;

                    cg = Data.CoeffGroups[coeff_index];
                    model1 = model2 = Util.GetSlice(Model.CoeffRact,pt,ct,cg);
                }
                LeftBlocks[Data.B6To4[b]].NotNullDc = AboveBlocks[AboveBlocksIdx[b]].NotNullDc = Convert.ToByte(BlockCoeff[b,0]!=0);
            }
        }

        internal Frame GetFrame()
        {
            return Frames[FrameSelect.CURRENT];
        }

        public void AddPredictorsDc(int ref_frame,int dequant_dc)
        {
            int idx = Data.Scantable[0];

            for(int b=0;b<6;++b)
            {
                ref Reference ab = ref AboveBlocks[AboveBlocksIdx[b]];
                ref Reference lb = ref LeftBlocks[Data.B6To4[b]];

                int count = 0, dc=0;

                if(ref_frame==lb.RefFrame)
                {
                    dc += lb.DcCoeff;
                    count++;
                }
                if(ref_frame==ab.RefFrame)
                {
                    dc += ab.DcCoeff;
                    count++;
                }
                if (count == 0)
                    dc = PrevDc[Data.B2p[b], ref_frame];
                else if (count == 2)
                    dc /= 2;

                BlockCoeff[b, idx] += (short)dc;
                PrevDc[Data.B2p[b], ref_frame] = BlockCoeff[b, idx];
                ab.DcCoeff = BlockCoeff[b, idx];
                ab.RefFrame = ref_frame;
                lb.DcCoeff = BlockCoeff[b, idx];
                lb.RefFrame = ref_frame;
                BlockCoeff[b, idx] *= (short)dequant_dc;
            }
        }

        public uint Width { get => m_width; set => m_width = value; }
        public uint Height { get => m_height; set => m_height = value; }
        public uint Denominator { get => m_denominator; set => m_denominator = value; }
        public uint Numerator { get => m_numerator; set => m_numerator = value; }
        public uint Framecount { get => m_framecount; set => m_framecount = value; }
        public StreamType Type { get => m_type; set => m_type = value; }
        internal RangeDecoder RangeDec { get => m_rangeDec; set => m_rangeDec = value; }
        public Profile Profile { get => m_profile; set => m_profile = value; }
        public bool UseLoopFiltering { get => m_useLoopFiltering; set => m_useLoopFiltering = value; }
        public bool LoopFilterSelector { get => m_loopFilterSelector; set => m_loopFilterSelector = value; }
        public Format Format { get => m_format; set => m_format = value; }
        internal Model Model { get => m_model; set => m_model = value; }
        internal Macroblock[] Macroblocks { get => m_macroblocks; set => m_macroblocks = value; }
        public short[,] PrevDc { get => m_prevDc; set => m_prevDc = value; }
        internal Frame[] Frames { get => m_frames; set => m_frames = value; }
        internal Reference[] AboveBlocks { get => m_aboveBlocks; set => m_aboveBlocks = value; }
        public uint YStride { get => m_yStride; set => m_yStride = value; }
        public uint UvStride { get => m_uvStride; set => m_uvStride = value; }
        public uint YSize { get => m_ySize; set => m_ySize = value; }
        public uint UvSize { get => m_uvSize; set => m_uvSize = value; }
        internal Reference[] LeftBlocks { get => m_leftBlocks; set => m_leftBlocks = value; }
        public int[] AboveBlocksIdx { get => m_aboveBlocksIdx; set => m_aboveBlocksIdx = value; }
        public uint MbWidth { get => m_mbWidth; set => m_mbWidth = value; }
        public uint MbHeight { get => m_mbHeight; set => m_mbHeight = value; }
        public int VectorCandidatePos { get => m_vectorCandidatePos; set => m_vectorCandidatePos = value; }
        internal Motionvector[] VectorCandidate { get => m_vectorCandidate; set => m_vectorCandidate = value; }
        public short[,] BlockCoeff { get => m_blockCoeff; set => m_blockCoeff = value; }
        internal RangeDecoder CoeffDec { get => m_coeffDec; set => m_coeffDec = value; }
        public CodingMode MacroblockType { get => m_macroblockType; set => m_macroblockType = value; }
        internal Motionvector[] Mvs { get => m_mvs; set => m_mvs = value; }
        internal IDCT Idct { get => m_idct; set => m_idct = value; }
        public int[] BlockOffset { get => m_blockOffset; set => m_blockOffset = value; }
        public int Flip { get => m_flip; set => m_flip = value; }
        public int Frbi { get => m_frbi; set => m_frbi = value; }
        public int Srbi { get => m_srbi; set => m_srbi = value; }
    }
}
