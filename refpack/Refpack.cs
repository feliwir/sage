using System;
using System.IO;

namespace sage.refpack
{
    public class Refpack
    {

        public static void Decompress(Stream input, Stream output)
        {
            BinaryReader reader;
            BinaryWriter writer;

            int proc_len, ref_dis, ref_len;

            if (input == null)
                throw new ArgumentNullException(nameof(input));
            reader = new BinaryReader(input);
            if (output == null)
                throw new ArgumentNullException(nameof(output));
            writer = new BinaryWriter(output);
            ushort signature = reader.ReadUInt16();
            int compressed_size = (signature & 0x0100) > 0 ? readU24(reader) : 0;
            int decompressed_size = readU24(reader);

            while (input.CanRead && output.CanWrite)
            {
                Console.WriteLine("Test");
                byte byte_0, byte_1, byte_2, byte_3;
                byte_0 = reader.ReadByte();
                if ((byte_0 & 0x80) == 0)
                {
                    /* 2-byte command: 0DDRRRPP DDDDDDDD */
                    byte_1 = reader.ReadByte();
                    proc_len = byte_0 & 0x03;
                    writer.Write(reader.ReadBytes(proc_len));

                    ref_dis = ((byte_0 & 0x60) << 3) + byte_1 + 1;
                    ref_len = ((byte_0 >> 2) & 0x07) + 3;
                    self_copy(reader, writer, ref_dis, ref_len);
                }
                else if ((byte_0 & 0x40) == 0)
                {
                    /* 3-byte command: 10RRRRRR PPDDDDDD DDDDDDDD */
                    byte_1 = reader.ReadByte();
                    byte_2 = reader.ReadByte();

                    proc_len = byte_1 >> 6;
                    writer.Write(reader.ReadBytes(proc_len));

                    ref_dis = ((byte_1 & 0x3f) << 8) + byte_2 + 1;
                    ref_len = (byte_0 & 0x3f) + 4;
                    self_copy(reader, writer, ref_dis, ref_len);
                }
                else if ((byte_0 & 0x20) == 0)
                {
                    /* 4-byte command: 110DRRPP DDDDDDDD DDDDDDDD RRRRRRRR*/
                    byte_1 = reader.ReadByte();
                    byte_2 = reader.ReadByte();
                    byte_3 = reader.ReadByte();

                    proc_len = byte_0 & 0x03;
                    writer.Write(reader.ReadBytes(proc_len));

                    ref_dis = ((byte_0 & 0x10) << 12) + (byte_1 << 8) + byte_2 + 1;
                    ref_len = ((byte_0 & 0x0c) << 6) + byte_3 + 5;
                    self_copy(reader, writer, ref_dis, ref_len);
                }
                else
                {
                    /* 1-byte command */
                    proc_len = (byte_0 & 0x1f);
                    if (proc_len <= 0x70)
                    {
                        /* no stop flag */
                        writer.Write(reader.ReadBytes(proc_len));
                    }
                    else
                    {
                        /* stop flag */
                        proc_len = byte_0 & 0x3;
                        writer.Write(reader.ReadBytes(proc_len));
                        break;
                    }
                }
                writer.Write(reader.ReadBytes(proc_len));
            }
        }

        private static void self_copy(BinaryReader reader, BinaryWriter writer, int dist, int len)
        {
            long stream_pos = reader.BaseStream.Position;
            reader.BaseStream.Seek(-dist, SeekOrigin.Current);
            writer.Write(reader.ReadBytes(len));
            reader.BaseStream.Seek(stream_pos, SeekOrigin.Begin);
        }


        private static int readU24(BinaryReader reader)
        {
            return (reader.ReadByte() << 16) | (reader.ReadByte() << 8) | reader.ReadByte();
        }

    }
}
