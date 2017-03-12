using System;
using System.IO;

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

    public struct FrameSelect
    {
        public const int NONE       = -1;
        public const int CURRENT    = 0;
        public const int PREVIOUS   = 1;
        public const int GOLDEN     = 2;
    }

    class Frame
    {     
        //Intra or Interframe
        private FrameType m_type;
        private bool m_isGolden;

        //Arithmetic coding 
        private int m_quantizer;
        private int m_dequant_ac;
        private int m_dequant_dc;
        private bool m_seperateCoeff;
        private ushort m_coeffOffset;

        //Fragments/ Amount of MBs
        private byte m_vfrags;
        private byte m_hfrags;
        
        //Calculated size
        private int m_dimX;
        private int m_dimY;
       
        //Output fragments
        private int m_ovfrags;
        private int m_ohfrags;
        
        //Calculated output size
        private int m_presX;
        private int m_presY;

        private int[] m_blockOffset;
        private bool m_useHuffman;

        private short[] m_planes;

        //Scaling mode
        ScalingMode m_scaling;

        //Read the FrameHeader
        public Frame(byte[] buf,Context c)
        {
            int index = 0;
            m_planes = new short[4];
            m_blockOffset = new int[6];
            m_type = (FrameType)((buf[index] >> 7) & 0x01);
            m_quantizer = (buf[index] >> 1) & 0x3F;
            
            //Get the quantizer values
            m_dequant_ac = Dequantizer.AC[m_quantizer] << 2;
            m_dequant_dc = Dequantizer.DC[m_quantizer] << 2;

            m_seperateCoeff = Convert.ToBoolean(buf[index] & 0x01);
            ++index;

            switch (m_type)
            {
                case FrameType.INTRA:
                    c.Format = (Format)(buf[index] >> 3);
                    c.Profile = (Profile)(buf[index] & 0x06);
                    ++index;

                    if(m_seperateCoeff || c.Profile == Profile.SIMPLE)
                    {
                        m_coeffOffset = (ushort)((buf[index++] << 8) | buf[index++]);
                    }

                    m_vfrags = buf[index++];
                    m_hfrags = buf[index++];

                    if(m_vfrags == 0|| m_hfrags == 0)
                    {
                        throw new InvalidDataException("Invalid size!");
                    }

                    m_dimX = m_hfrags * 16;
                    m_dimY = m_vfrags * 16;

                    m_ovfrags = buf[index++];
                    m_ohfrags = buf[index++];

                    m_presX = m_ohfrags * 16;
                    m_presY = m_ovfrags * 16;

                    //check if size changed
                    if(m_dimX!=c.Width||m_dimY!=c.Height||c.Macroblocks is null)
                    {
                        c.MbWidth = m_hfrags;
                        c.MbHeight = m_vfrags;
                        c.Width = (uint)m_dimX;
                        c.Height = (uint)m_dimY;
                        //Allocate the Macroblocks
                        c.Macroblocks = new Macroblock[m_vfrags * m_hfrags];
                        for (int i = 0; i < c.Macroblocks.Length; ++i)
                            c.Macroblocks[i] = new Macroblock();

                        //Allocate the above Blocks
                        c.AboveBlocks = new Reference[4 * m_hfrags + 6];

                        //Set stride & size
                        c.YStride = c.Width;
                        c.UvStride = c.YStride / 2;
                        c.YSize = c.YStride * (c.Height);
                        c.UvSize = c.YSize / 4;

                    }

                    c.RangeDec = new RangeDecoder(buf,index);
                    //this is the scaling mode
                    m_scaling = (ScalingMode)c.RangeDec.ReadBits(2);
                    m_isGolden = false;
                    break;
                case FrameType.INTER:
                    if (m_seperateCoeff || c.Profile == Profile.SIMPLE)
                    {
                        m_coeffOffset = (ushort)((buf[index++] << 8) | buf[index++]);
                    }

                    c.RangeDec = new RangeDecoder(buf, index);
                    m_isGolden = Convert.ToBoolean(c.RangeDec.ReadBit());
                    if(c.Profile==Profile.ADVANCED)
                    {
                        c.UseLoopFiltering = Convert.ToBoolean(c.RangeDec.ReadBit());
                        if(c.UseLoopFiltering)
                            c.LoopFilterSelector = Convert.ToBoolean(c.RangeDec.ReadBit());
                    }
                    break;
            }

            if(c.Profile==Profile.ADVANCED || c.Format==Format.VP62)
            {
                throw new NotImplementedException("Not implemented VP62 yet");
            }

            m_useHuffman = Convert.ToBoolean(c.RangeDec.ReadBit());

            if(m_coeffOffset>0)
            {
                if (m_useHuffman)
                {

                }
                else
                {
                    c.CoeffDec = new RangeDecoder(buf, m_coeffOffset);
                }
            }
            else
            {
                c.CoeffDec = c.RangeDec;
            }
        }

        public void Decode(Context c)
        {

            if(m_type==FrameType.INTRA)
            {
                c.Model.Default();
                //All macroblocks are intra frames
                foreach(var mb in c.Macroblocks)
                {
                    mb.Type = CodingMode.INTRA;
                }
            }
            else
            {
                
            }

            ParseCoeffModels(c);

            for (int i = 0; i < 3; ++i)
                for (int j = 0; j < 3; ++j)
                    c.PrevDc[i, j] = 0;

            c.PrevDc[1, FrameSelect.CURRENT] = 128;
            c.PrevDc[2, FrameSelect.CURRENT] = 128;

            for (int block = 0; block < 4 * c.MbWidth + 6; block++)
            {
                c.AboveBlocks[block].RefFrame = FrameSelect.NONE;
                c.AboveBlocks[block].DcCoeff = 0;
                c.AboveBlocks[block].NotNullDc = 0;
            }

            c.AboveBlocks[2 * c.MbWidth + 2].RefFrame = FrameSelect.CURRENT;
            c.AboveBlocks[3 * c.MbWidth + 4].RefFrame = FrameSelect.CURRENT;

            //The loop for decoding each Macroblock
            for(int row=0;row<c.MbHeight;++row)
            {
                for(int block=0;block<4;++block)
                {
                    c.LeftBlocks[block].RefFrame = FrameSelect.NONE;
                    c.LeftBlocks[block].DcCoeff = 0;
                    c.LeftBlocks[block].NotNullDc = 0;
                }

                c.AboveBlocksIdx[0] = 1;
                c.AboveBlocksIdx[1] = 2;
                c.AboveBlocksIdx[2] = 1;
                c.AboveBlocksIdx[3] = 2;
                c.AboveBlocksIdx[4] = 2 * (int)c.MbWidth + 2 + 1;
                c.AboveBlocksIdx[5] = 3 * (int)c.MbWidth + 4 + 1;

                //calculate the pixeloffset for each block
                BlockOffset[0] = (int)(row * c.YStride * 16);      //UPPER LEFT
                BlockOffset[1] = BlockOffset[0] + 8;                    //UPPER RIGHT
                BlockOffset[2] = (int)(BlockOffset[0] + 8 * c.YStride); //LOWER LEFT
                BlockOffset[3] = BlockOffset[2] + 8;                    //LOWER RIGHT
                BlockOffset[4] = (int)(row * 8 * c.UvStride);      //OFFSET IN U PLANE
                BlockOffset[5] = BlockOffset[4];                        //OFFSET IN V PLANE

                for(int column=0;column<c.MbWidth;++column)
                {
                    DecodeMacroblock(c, row, column);
                }
            }          
        }

        private void DecodeMacroblock(Context c,int row,int column)
        {
            CodingMode mode;
            int ref_frame,ab,b_max;
            if (m_type==FrameType.INTRA)
            {
                mode = CodingMode.INTRA;
            }
            else
            {
                mode = DecodeMotionvector(c, row, column);
            }

            ref_frame = Data.ReferenceFrame[(int)mode];

            c.ParseCoefficients(m_dequant_ac);

            c.AddPredictorsDc(ref_frame, m_dequant_dc);

            Frame frame_ref = c.Frames[ref_frame];

            //TODO: work here
        }

        static private int GetVectorPredictors(Context c, int row, int column, int ref_frame)
        {
            int nb_preds = 0;
            Motionvector[] vect = new Motionvector[2];
            Motionvector mvp = new Motionvector();
            int offset = 0;

            for (int pos = 0; pos < 12; ++pos)
            {
                mvp.X = column + Data.CandidatePredictorPos[pos, 0];
                mvp.Y = row + Data.CandidatePredictorPos[pos, 1];

                if (mvp.X < 0 || mvp.X >= c.MbWidth ||
                    mvp.Y < 0 || mvp.Y >= c.MbHeight)
                    continue;

                offset = (int)(mvp.X + c.MbWidth * mvp.Y);

                //this is not the frametype we are looking for, so skip the rest of this loop
                if (Data.ReferenceFrame[(int)c.Macroblocks[offset].Type] != ref_frame)
                    continue;

                //this motion vector is either same as last frame or (0,0)
                if ((c.Macroblocks[offset].Mv.X == vect[0].X &&
                     c.Macroblocks[offset].Mv.Y == vect[0].Y) ||
                    (c.Macroblocks[offset].Mv.X == 0 &&
                     c.Macroblocks[offset].Mv.Y == 0))
                    continue;

                vect[nb_preds++] = c.Macroblocks[offset].Mv;
                if(nb_preds>1)
                {
                    nb_preds = -1;
                    break;
                }

                c.VectorCandidatePos = pos;
            }

            c.VectorCandidate = vect;
            return nb_preds;
        }

        private CodingMode DecodeMotionvector(Context c,int row,int column)
        {
            int ctx = GetVectorPredictors(c, row, column, FrameSelect.PREVIOUS);


            

            return CodingMode.INTER_MV;
        }

        private void ParseCoeffModels(Context c)
        {
            int[] DefaultProb = new int[11];

            for(int i=0;i<DefaultProb.Length;++i)
                DefaultProb[i] = 0x80;

            for(int pt=0;pt<2;++pt)
            {
                for(int node=0;node<11;++node)
                {
                    if(c.RangeDec.GetBitProbabilityBranch(Data.DccvPct[pt,node])>0)
                    {
                        DefaultProb[node] = c.RangeDec.ReadBitsNn(7);
                        c.Model.CoeffDccv[pt,node] = (byte)DefaultProb[node];
                    }
                    else if(m_type==FrameType.INTRA)
                    {
                        c.Model.CoeffDccv[pt, node] = (byte)DefaultProb[node];
                    }
                }
            }

            if(Convert.ToBoolean(c.RangeDec.ReadBit()))
            {
                for(int pos=1;pos<64;++pos)
                {
                    if (c.RangeDec.GetBitProbabilityBranch(Data.CoeffReorderPct[pos]) > 0)
                    {
                        c.Model.CoeffReorder[pos] = (byte)c.RangeDec.ReadBits(4);
                    }
                }

                c.Model.InitializeCoeffOrderTable();
            }

            for(int cg=0;cg<2;++cg)
            {
                for(int node=0;node<14;++node)
                {
                    if(c.RangeDec.GetBitProbabilityBranch(Data.RunvPct[cg,node]) > 0)
                    {
                        c.Model.CoeffRunv[cg,node] = (byte)c.RangeDec.ReadBitsNn(7); 
                    }
                }
            }

            for (int ct = 0; ct < 3; ct++)
            {
                for (int pt = 0; pt < 2; pt++)
                {
                    for (int cg = 0; cg < 6; cg++)
                    {
                        for (int node = 0; node < 11; node++)
                        {
                            if (c.RangeDec.GetBitProbabilityBranch(Data.RactPct[ct, pt, cg, node]) > 0)
                            {
                                DefaultProb[node] = c.RangeDec.ReadBitsNn(7);
                                c.Model.CoeffRact[pt, ct, cg, node] = (byte)DefaultProb[node];
                            }
                            else if (m_type==FrameType.INTRA)
                            {
                                c.Model.CoeffRact[pt, ct, cg, node] = (byte)DefaultProb[node];
                            }
                        }                         
                    }                      
                }                  
            }
            
            if(m_useHuffman)
            {
                
            }
            else
            {
                //Calculate DCCT
                for (int pt = 0; pt < 2; pt++)
                    for (int ctx = 0; ctx < 3; ctx++)
                        for (int node = 0; node < 5; node++)
                            c.Model.CoeffDcct[pt,ctx,node] = (byte)Util.Clip(((c.Model.CoeffDccv[pt,node] * Data.DccvLc[ctx,node,0] + 128) >> 8) + Data.DccvLc[ctx,node,1], 1, 255);
            }
        }

        public FrameType Type { get => m_type; set => m_type = value; }
        public bool IsGolden { get => m_isGolden; set => m_isGolden = value; }
        public int[] BlockOffset { get => m_blockOffset; set => m_blockOffset = value; }
        public bool UseHuffman { get => m_useHuffman; set => m_useHuffman = value; }
    }
}
