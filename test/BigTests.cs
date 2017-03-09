using Xunit;
using sage.big;
using System.IO;

namespace test
{
    public class BigTests
    {
        [Fact]
        public void Seeking()
        {
            BigArchive archive = new BigArchive(File.Open("test.big", FileMode.Open, FileAccess.Read, FileShare.Read));
            foreach (var e in archive.Entries)
            {
                Stream s = e.Open();
                Assert.Equal(s.Position, 0);
                s.Seek(0, SeekOrigin.End);
                Assert.Equal(s.Position, s.Length);
            }
            archive.Dispose();
        }

        [Fact]
        public void Open()
        {
            BigArchive archive = new BigArchive(File.Open("test.big",FileMode.Open,FileAccess.Read,FileShare.Read));
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
            BigArchive archive = new BigArchive(File.Open("test.big", FileMode.Open, FileAccess.Read, FileShare.Read));
            archive.Dispose();
        }
    }
}
