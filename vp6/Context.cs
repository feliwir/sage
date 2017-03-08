using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace sage.vp6
{
    //Decoder Context for VP6
    public class Context
    {
        //STREAM INFORMATION
        private uint m_width;
        private uint m_height;
        private uint m_denominator;
        private uint m_numerator;
        private uint m_framecount;
        private StreamType m_type;
        
        //REQUIRED FOR DECODING 
        private Frame m_golden;
        private Frame m_previous;
        private Frame m_current;
        private Model m_model;
        private BitReader m_rangeDec;
        private BitReader m_huffDec;
        private Profile m_profile;
        private bool m_useLoopFiltering;
        private bool m_loopFilterSelector;
        private Format m_format;
        private bool m_useHuffman;
        private Macroblock[] m_macroblocks;

        public Context(uint width, uint height, uint denominator, uint numerator,
            uint framecount, StreamType type)
        {
            Width = width;
            Height = height;
            Denominator = denominator;
            Numerator = numerator;
            Framecount = framecount;
            Type = type;
            Model = new Model();
        }

        /// <summary>
        /// Tells if this decoder is requiring a new packet
        /// </summary>
        /// <returns>Is a packet required</returns>
        public bool RequirePacket()
        {
            return true;
        }

        public void ProcessPacket(BinaryReader br, int packet_size)
        {
            m_previous = m_current;
            //Substract the first 8 bytes that have already been read
            byte[] buffer = br.ReadBytes(packet_size - 8);
            
            //One packet is always exactly one frame
            Frame frame = new Frame(buffer, this);

            //Decode this frame
            frame.Decode(this);

            //This is a golden frame, so update it
            if (frame.IsGolden)
            {
                m_golden = frame;
            }

            m_current = frame;
        }

        public uint Width { get => m_width; set => m_width = value; }
        public uint Height { get => m_height; set => m_height = value; }
        public uint Denominator { get => m_denominator; set => m_denominator = value; }
        public uint Numerator { get => m_numerator; set => m_numerator = value; }
        public uint Framecount { get => m_framecount; set => m_framecount = value; }
        public StreamType Type { get => m_type; set => m_type = value; }
        internal BitReader RangeDec { get => m_rangeDec; set => m_rangeDec = value; }
        public Profile Profile { get => m_profile; set => m_profile = value; }
        public bool UseLoopFiltering { get => m_useLoopFiltering; set => m_useLoopFiltering = value; }
        public bool LoopFilterSelector { get => m_loopFilterSelector; set => m_loopFilterSelector = value; }
        public Format Format { get => m_format; set => m_format = value; }
        public bool UseHuffman { get => m_useHuffman; set => m_useHuffman = value; }
        internal BitReader HuffDec { get => m_huffDec; set => m_huffDec = value; }
        internal Model Model { get => m_model; set => m_model = value; }
        internal Macroblock[] Macroblocks { get => m_macroblocks; set => m_macroblocks = value; }
    }
}
