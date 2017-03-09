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
        
 
        //Scaling mode
        ScalingMode m_scaling;

        //Read the FrameHeader
        public Frame(byte[] buf,Context c)
        {
            int index = 0;
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
                        m_coeffOffset = (ushort)((buf[2] << 8) | buf[3]);
                        index += 2;
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
                        c.Width = (uint)m_dimX;
                        c.Height = (uint)m_dimY;
                        //Allocate the Macroblocks
                        c.Macroblocks = new Macroblock[m_vfrags * m_hfrags];
                        for (int i = 0; i < c.Macroblocks.Length; ++i)
                            c.Macroblocks[i] = new Macroblock();

                    }

                    c.RangeDec = new RangeDecoder(buf,index);
                    //this is the scaling mode
                    m_scaling = (ScalingMode)c.RangeDec.ReadBits(2);
                    m_isGolden = false;
                    break;
                case FrameType.INTER:
                    if (m_seperateCoeff || c.Profile == Profile.SIMPLE)
                    {
                        m_coeffOffset = BitConverter.ToUInt16(buf, index);
                        index += 2;
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

            c.UseHuffman = Convert.ToBoolean(c.RangeDec.ReadBit());

            if(m_coeffOffset>0)
            {
                c.HuffDec = new RangeDecoder(buf, m_coeffOffset);
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
        }

        private void ParseCoeffModels(Context c)
        {
            int[] DefaultProbe = new int[11];

            for(int i=0;i<DefaultProbe.Length;++i)
                DefaultProbe[i] = 0x80;

            for(int pt=0;pt<2;++pt)
            {
                for(int node=0;node<11;++node)
                {
                    
                }
            }
        }

        private int ReadBitProbability(int prob)
        {
           
            return 0;
        }

        public FrameType Type { get => m_type; set => m_type = value; }
        public bool IsGolden { get => m_isGolden; set => m_isGolden = value; }
    }
}
