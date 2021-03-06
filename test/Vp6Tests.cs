﻿using Xunit;
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
            Assert.Equal((decimal)demuxer.Video.Width, 640);
            Assert.Equal((decimal)demuxer.Video.Height, 480);           
        }
    }
}
