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
            Refpack.Decompress(File.Open("compressed.txt", FileMode.Open), File.Open("test.txt", FileMode.Open));
        }
    }
}
