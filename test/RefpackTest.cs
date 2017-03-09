using System.IO;
using Xunit;
using sage.refpack;
using System;

namespace test
{
    public class RefpackTest
    {
        [Fact]
        public void Decompress()
        {
            Console.WriteLine(Directory.GetCurrentDirectory());
            Decompressor dc = new Decompressor(File.Open("compressed.txt", FileMode.Open));
            MemoryStream ms = dc.Decompress();
            ms.WriteTo(File.Create("test.txt"));
        }
    }
}
