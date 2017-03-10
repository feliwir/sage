<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;
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

        private List<byte> test;

        public bool IsCompressed { get => is_compressed; set => is_compressed = value; }
        public int UncompressedLength { get => uncompressed_length; set => uncompressed_length = value; }

        public Decompressor(Stream stream)
        {
            br = new BinaryReader(stream);
            ReadHeader();
            test = new List<byte>();
        }

        public MemoryStream Decompress()
        {
            ms = new MemoryStream(UncompressedLength);
            bw = new BinaryWriter(ms);

            if (IsCompressed)
            {
                while (!stop)
                {
                    DecompressionOneStep();
                }
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
            var str = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            int i = 0;
        }

        void CopyShort()
        {
            prefix[1] = br.ReadByte();
            long num_src_bytes = prefix[0] & 3;
            long num_dst_bytes = ((prefix[0] & 0x1C) >> 2) + 3;
            long dst_offset = (((prefix[0] & 0x60) << 3) | prefix[1]) + 1;
            long pos = br.BaseStream.Position;
            byte[] data = br.ReadBytes((int)num_src_bytes);
            test.AddRange(data);
            bw.Write(data);
            long pos2 = br.BaseStream.Position;
            var str = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            Copy(dst_offset, num_dst_bytes);
        }

        void CopyMedium()
        {
            prefix[1] = br.ReadByte();
            prefix[2] = br.ReadByte();
            long num_src_bytes = prefix[1] >> 6;
            long num_dst_bytes = (prefix[0] & 0x3F) + 4;
            long dst_offset = (((prefix[1] & 0x3F) << 8) | prefix[2]) + 1;
            byte[] data = br.ReadBytes((int)num_src_bytes);
            test.AddRange(data);
            bw.Write(data);
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
            byte[] data = br.ReadBytes((int)num_src_bytes);
            test.AddRange(data);
            bw.Write(data);
            Copy(dst_offset, num_dst_bytes);
        }

        void ImmediateBytesAndFinish()
        {
            long num_src_bytes = prefix[0] & 3;
            byte[] data = br.ReadBytes((int)num_src_bytes);
            test.AddRange(data);
            bw.Write(data);
            stop = true;
        }

        void ImmediateBytesLong()
        {
            long num_src_bytes = ((prefix[0] & 0x1F) + 1) * 4;
            byte[] data = br.ReadBytes((int)num_src_bytes);
            test.AddRange(data);
            bw.Write(data);
        }

        void Copy(long offset, long length)
        {
            //long pos = br.BaseStream.Position;
            //br.BaseStream.Seek(-offset, SeekOrigin.Current);
            //long pos_new = br.BaseStream.Position;
            //byte[] data = br.ReadBytes((int)length);
            long size = test.Count;
            int idx = (int)(size - offset);
            for (int i = 0; i < length; i++)
            { 
                bw.Write(test[idx + i]);
                test.Add(test[idx + i]);
            }
            //bw.Write(data);
            //var str = System.Text.Encoding.UTF8.GetString(data);
            //bw.Write(data);
            //br.BaseStream.Seek(pos , SeekOrigin.Begin);
        }

        void Pump(int count)
        {
            byte[] data = br.ReadBytes(count);
            test.AddRange(data);
            bw.Write(data);
        }

        private void ReadHeader()
        {
            byte[] hdr = br.ReadBytes(2);

            if ((hdr[0] & 0x3E) == 0x10 && (hdr[1] == 0xFB))
            {
                IsCompressed = true;

                bool is_long = ((hdr[0] & 0x80) != 0);
                bool has_more = ((hdr[0] & 0x01) != 0);

                byte[] buffer = br.ReadBytes((is_long ? 4 : 3) * (has_more ? 2 : 1));

                uncompressed_length = (((buffer[0] << 8) + buffer[1]) << 8) + buffer[2];
                if (is_long)
                {
                    uncompressed_length = (uncompressed_length << 8) + buffer[3];
                }
            }
        }

        private int ReadU24()
        {
            return (br.ReadByte() << 16) | (br.ReadByte() << 8) | br.ReadByte();
        }

    }
}
