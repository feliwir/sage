using System;
using System.Collections.Generic;
using System.Text;

namespace sage.vp6
{
    class PixelFormat
    {
        public static byte[] Yuv420pToRGB(byte[] y,byte[] u, byte[] v,uint width,uint height)
        {
            uint size = width * height;
            //Allocate the RGB buffer
            byte[] buffer = new byte[size * 3];

            int uv_index = 0, pass = 0;
            int b, g, r;
            int rgb_index = 0;
            for (int px=0; px<size/6;++px)
            {
                if (pass == 2)
                {
                    pass = 0;
                    uv_index += 3;
                }

                int y1 = y[px];
                int y2 = y[px + 1];
                int y3 = y[px + 2];
                int y4 = y[px + 3];
                int y5 = y[px + 4];
                int y6 = y[px + 5];

                int u1 = u[uv_index];
                int u2 = u[uv_index + 1];
                int u3 = u[uv_index + 2];

                int v1 = v[uv_index];
                int v2 = v[uv_index + 1];
                int v3 = v[uv_index + 2];
                
                //1
                r = (int)(1.164 * (y1 - 16) + 1.596 * (v1 - 128));
                g = (int)(1.164 * (y1 - 16) - 0.813 * (v1 - 128) - 0.391 * (u1 - 128));
                b = (int)(1.164 * (y1 - 16) + 2.018 * (u1 - 128));
                buffer[rgb_index++] = Util.ClipByte(r);
                buffer[rgb_index++] = Util.ClipByte(g);
                buffer[rgb_index++] = Util.ClipByte(b);
                //2
                r = (int)(1.164 * (y2 - 16) + 1.596 * (v1 - 128));
                g = (int)(1.164 * (y2 - 16) - 0.813 * (v1 - 128) - 0.391 * (u1 - 128));
                b = (int)(1.164 * (y2 - 16) + 2.018 * (u1 - 128));
                buffer[rgb_index++] = Util.ClipByte(r);
                buffer[rgb_index++] = Util.ClipByte(g);
                buffer[rgb_index++] = Util.ClipByte(b);
                //3
                r = (int)(1.164 * (y3 - 16) + 1.596 * (v2 - 128));
                g = (int)(1.164 * (y3 - 16) - 0.813 * (v2 - 128) - 0.391 * (u2 - 128));
                b = (int)(1.164 * (y3 - 16) + 2.018 * (u2 - 128));
                buffer[rgb_index++] = Util.ClipByte(r);
                buffer[rgb_index++] = Util.ClipByte(g);
                buffer[rgb_index++] = Util.ClipByte(b);
                //4
                r = (int)(1.164 * (y4 - 16) + 1.596 * (v2 - 128));
                g = (int)(1.164 * (y4 - 16) - 0.813 * (v2 - 128) - 0.391 * (u2 - 128));
                b = (int)(1.164 * (y4 - 16) + 2.018 * (u2 - 128));
                buffer[rgb_index++] = Util.ClipByte(r);
                buffer[rgb_index++] = Util.ClipByte(g);
                buffer[rgb_index++] = Util.ClipByte(b);
                //5
                r = (int)(1.164 * (y5 - 16) + 1.596 * (v3 - 128));
                g = (int)(1.164 * (y5 - 16) - 0.813 * (v3 - 128) - 0.391 * (u3 - 128));
                b = (int)(1.164 * (y5 - 16) + 2.018 * (u3 - 128));
                buffer[rgb_index++] = Util.ClipByte(r);
                buffer[rgb_index++] = Util.ClipByte(g);
                buffer[rgb_index++] = Util.ClipByte(b);
                //6
                r = (int)(1.164 * (y6 - 16) + 1.596 * (v3 - 128));
                g = (int)(1.164 * (y6 - 16) - 0.813 * (v3 - 128) - 0.391 * (u3 - 128));
                b = (int)(1.164 * (y6 - 16) + 2.018 * (u3 - 128));
                buffer[rgb_index++] = Util.ClipByte(r);
                buffer[rgb_index++] = Util.ClipByte(g);
                buffer[rgb_index++] = Util.ClipByte(b);

                pass++;
            }

            return buffer;
        }

    }
}
