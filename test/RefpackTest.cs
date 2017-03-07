using System.IO;
using Xunit;
using sage.refpack;

namespace test
{
    public class RefpackTest
    {
        [Fact]
        public void Decompress()
        {
            Decompressor dc = new Decompressor(File.Open("test/compressed.txt", FileMode.Open));
            MemoryStream ms = dc.Decompress();
            using (var fileStream = new FileStream("test/test.txt", FileMode.Create, FileAccess.Write))
            {
                ms.CopyTo(fileStream);
            }
        }
    }
}
