using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace sage.vp6
{
    //Decoder Context for VP6
    public class Decoder
    {
        private uint m_width;
        private uint m_height;
        private uint m_denominator;
        private uint m_numerator;
        private uint m_framecount;
        private StreamType m_type;
        private Frame m_golden;


        public uint Width { get => m_width; set => m_width = value; }
        public uint Height { get => m_height; set => m_height = value; }
        public uint Denominator { get => m_denominator; set => m_denominator = value; }
        public uint Numerator { get => m_numerator; set => m_numerator = value; }
        public uint Framecount { get => m_framecount; set => m_framecount = value; }
        public StreamType Type { get => m_type; set => m_type = value; }

        public Decoder(uint width, uint height, uint denominator,uint numerator,
            uint framecount, StreamType type)
        {
            Width = width;
            Height = height;
            Denominator = denominator;
            Numerator = numerator;
            Framecount = framecount;
            Type = type;
        }

        /// <summary>
        /// Tells if this decoder is requiring a new packet
        /// </summary>
        /// <returns>Is a packet required</returns>
        public bool RequirePacket()
        {
            return true;
        }

        public void ProcessPacket(BinaryReader br,int packet_size)
        {
            //Substract the first 8 bytes that have already been read
            byte[] buffer =  br.ReadBytes(packet_size-8);
            Frame frame = new Frame(buffer);
           
        }

    }
}
