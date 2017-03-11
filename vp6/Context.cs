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
        private uint m_denominator;
        private uint m_numerator;
        private uint m_framecount;
        private StreamType m_type;

        //REQUIRED FOR DECODING 
        private Frame[] m_frames;
        private Model m_model;
        private RangeDecoder m_rangeDec;
        private RangeDecoder m_huffDec;
        private Profile m_profile;
        private bool m_useLoopFiltering;
        private bool m_loopFilterSelector;
        private Format m_format;
        private Macroblock[] m_macroblocks;
        private Reference[] m_aboveBlocks;
        private Reference[] m_leftBlocks;
        private int[] m_aboveBlocksIdx;

        //REQUIRED for prediction
        private short[,] m_prevDc;
        //Information regarding the frames
        private uint m_yStride;
        private uint m_uvStride;
        private uint m_ySize;
        private uint m_uvSize;

        public Context(uint width, uint height, uint denominator, uint numerator,
            uint framecount, StreamType type)
        {
            Width = width;
            Height = height;
            Denominator = denominator;
            Numerator = numerator;
            Framecount = framecount;
            Type = type;
            Frames = new Frame[3];
            LeftBlocks = new Reference[4];
            Model = new Model();
            PrevDc = new short[3, 3];
            AboveBlocksIdx = new int[6];
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
            Frames[FrameSelect.PREVIOUS] = Frames[FrameSelect.CURRENT];
            //Substract the first 8 bytes that have already been read
            byte[] buffer = br.ReadBytes(packet_size - 8);
            
            //One packet is always exactly one frame
            Frame frame = new Frame(buffer, this);

            //Decode this frame
            frame.Decode(this);

            //This is a golden frame, so update it
            if (frame.IsGolden)
            {
                Frames[FrameSelect.GOLDEN] = frame;
            }

            Frames[FrameSelect.CURRENT] = frame;
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
        internal RangeDecoder HuffDec { get => m_huffDec; set => m_huffDec = value; }
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
    }
}
