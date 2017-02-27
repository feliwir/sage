using System;
using Xunit;
using sage.big;
using System.IO;

namespace test
{
    public class BigTests
    {
        [Fact]
        public void Open()
        {
            BigArchive archive = new BigArchive(File.Open("test.big",FileMode.Open));
            Assert.Equal(1, 1);
        }

        [Fact]
        public void Create()
        {
            BigArchive archive = new BigArchive(File.Open("test.big",FileMode.Create),BigArchiveMode.Create);
        }
    }
}
