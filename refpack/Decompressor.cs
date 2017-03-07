﻿using System;
using System.IO;

namespace sage.refpack
{
    public class Decompressor
    {
        private BinaryReader br;
        private MemoryStream ms;
        private BinaryWriter bw;

        private bool is_compressed = false;
        private Int32 uncompressed_length = 0;

        private bool stop = false;
        private byte[] prefix = new byte[4];

        public Decompressor(Stream stream)
        {
            br = new BinaryReader(stream);
            readHeader();
        }

        public MemoryStream Decompress()
        {
            ms = new MemoryStream(uncompressed_length);
            bw = new BinaryWriter(ms);

            if (is_compressed)
            {
                while (!stop)
                    DecompressionOneStep();
            }
            else
            {
                //just copy everything to return value
                //or should we throw an error?
                bw.Write(br.ReadBytes((int)br.BaseStream.Length));
            }

            return ms;
        }

        private void DecompressionOneStep()
        {
            prefix[0] = br.ReadByte();
            if (prefix[0] >= 0xC0)
            {
                if (prefix[0] >= 0xE0)
                {
                    if (prefix[0] >= 0xFC)
                    {
                        ImmediateBytesAndFinish();
                    }
                    else
                    {
                        ImmediateBytesLong();
                    }
                }
                else
                {
                    CopyLong();
                }
            }
            else
            {
                if (prefix[0] >= 0x80)
                {
                    CopyMedium();
                }
                else
                {
                    CopyShort();
                }
            }
        }

        void CopyShort()
        {
            prefix[1] = br.ReadByte();
            long num_src_bytes = prefix[0] & 3;
            long num_dst_bytes = ((prefix[0] & 0x1C) >> 2) + 3;
            long dst_offset = (((prefix[0] & 0x60) << 3) | prefix[1]) + 1;
            bw.Write(br.ReadBytes((int)num_src_bytes));
            Copy(dst_offset, num_dst_bytes);
        }

        void CopyMedium()
        {
            prefix[1] = br.ReadByte();
            prefix[2] = br.ReadByte();
            long num_src_bytes = prefix[1] >> 6;
            long num_dst_bytes = (prefix[0] & 0x3F) + 4;
            long dst_offset = (((prefix[1] & 0x3F) << 8) | prefix[2]) + 1;
            bw.Write(br.ReadBytes((int)num_src_bytes));
            Copy(dst_offset, num_dst_bytes);
        }

        void CopyLong()
        {
            prefix[1] = br.ReadByte();
            prefix[2] = br.ReadByte();
            prefix[3] = br.ReadByte();
            long num_src_bytes = prefix[0] & 3;
            long num_dst_bytes = (((prefix[0] & 0x0C) << 6) | prefix[3]) + 5;
            long dst_offset = (((((prefix[0] & 0x10) << 4) | prefix[1]) << 8) | prefix[2]) + 1;
            bw.Write(br.ReadBytes((int)num_src_bytes));
            Copy(dst_offset, num_dst_bytes);
        }

        void ImmediateBytesAndFinish()
        {
            long num_src_bytes = prefix[0] & 3;
            bw.Write(br.ReadBytes((int)num_src_bytes));
            stop = true;
        }

        void ImmediateBytesLong()
        {
            long num_src_bytes = ((prefix[0] & 0x1F) + 1) * 4;
            bw.Write(br.ReadBytes((int)num_src_bytes));
        }

        void Copy(long offset, long length)
        {
            long pos = br.BaseStream.Position;
            long off = (pos - offset) % br.BaseStream.Length;
            br.BaseStream.Seek(off, SeekOrigin.Current);
            bw.Write(br.ReadBytes((int)length));
            br.BaseStream.Seek(pos, SeekOrigin.Begin);
        }

        private void readHeader()
        {
            byte[] data = br.ReadBytes(6);
            UInt16 hdr = (UInt16)((data[0] << 8) | data[1]);
 
            if ((hdr & 0x3EFF) == 0x10FB)
            {
                is_compressed = true;
                uncompressed_length = (((data[2] << 8) | data[3]) | data[4]);
                if ((data[0] & 0x80) != 0)
                {
                    uncompressed_length = (uncompressed_length << 8) | data[5];
                }
            }
        }

        public Int32 GetUncompressedSize()
        {
            return uncompressed_length;
        }

        public bool IsCompressed()
        {
            return is_compressed;
        }

        private int readU24()
        {
            return (br.ReadByte() << 16) | (br.ReadByte() << 8) | br.ReadByte();
        }

    }
}