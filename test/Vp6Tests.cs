using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using sage.vp6;
using System.IO;

namespace test
{
    public class Vp6Tests
    {
        [Fact]
        public void Demuxer()
        {
            Demuxer demuxer = new Demuxer(File.Open("test.vp6", FileMode.Open));
            Assert.Equal((decimal)demuxer.Video.Width, 64);
            Assert.Equal((decimal)demuxer.Video.Height, 64);
            
        }
    }
}
