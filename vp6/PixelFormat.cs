using System;
using System.Collections.Generic;
using System.Text;

namespace sage.vp6
{
    class PixelFormat
    {
        public static byte[] Yuv420pToRGB(byte[] ybuf, byte[] ubuf, byte[] vbuf, uint width, uint height)
        {
            uint size = width * height;
            //Allocate the RGB buffer
            byte[] buffer = new byte[size * 3];

            for (int row = 0; row < height; ++row)
            {
                for (int col = 0; col < width; ++col)
                {
                    //get YUV values
                    ref byte y = ref ybuf[row * width + col];
                    ref byte u = ref ubuf[(row / 2) * (width / 2) + col / 2];
                    ref byte v = ref vbuf[(row / 2) * (width / 2) + col / 2];

                    //get RGB values
                    ref byte r = ref buffer[(row * width + col) * 3];
                    ref byte g = ref buffer[(row * width + col) * 3 + 1];
                    ref byte b = ref buffer[(row * width + col) * 3 + 2];

                    //perform the conversion
                    b = (byte)(1.164*(y - 16) + 2.018*(u - 128));
                    g = (byte)(1.164 *(y - 16) - 0.813*(v - 128) - 0.391*(u - 128));
                    r = (byte)(1.164 *(y - 16) + 1.596*(v - 128));
                }
            }
            return buffer;
        }

    }
}
