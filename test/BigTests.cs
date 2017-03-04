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
            Assert.Equal(archive.Entries.Count, 2);
            foreach(var e in archive.Entries)
            {
                StreamReader sr = new StreamReader(e.Open());
                sr.ReadToEnd();
            }
            archive.Dispose();
        }

        [Fact]
        public void Create()
        {
            BigArchive archive = new BigArchive(File.Open("test.big",FileMode.Create),BigArchiveMode.Create);
            archive.Dispose();
        }
    }
}
