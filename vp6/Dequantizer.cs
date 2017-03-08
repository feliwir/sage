﻿using System;
using System.Collections.Generic;
using System.Text;

namespace sage.vp6
{
    class Dequantizer
    {
        public static readonly byte[] AC = new byte[] { 94, 92, 90, 88, 86, 82, 78, 74,
                                                        70, 66, 62, 58, 54, 53, 52, 51,
                                                        50, 49, 48, 47, 46, 45, 44, 43,
                                                        42, 40, 39, 37, 36, 35, 34, 33,
                                                        32, 31, 30, 29, 28, 27, 26, 25,
                                                        24, 23, 22, 21, 20, 19, 18, 17,
                                                        16, 15, 14, 13, 12, 11, 10,  9,
                                                         8,  7,  6,  5,  4,  3,  2,  1 };

        public static readonly byte[] DC = new byte[] { 47, 47, 47, 47, 45, 43, 43, 43,
                                                        43, 43, 42, 41, 41, 40, 40, 40,
                                                        40, 35, 35, 35, 35, 33, 33, 33,
                                                        33, 32, 32, 32, 27, 27, 26, 26,
                                                        25, 25, 24, 24, 23, 23, 19, 19,
                                                        19, 19, 18, 18, 17, 16, 16, 16,
                                                        16, 16, 15, 11, 11, 11, 10, 10,
                                                         9,  8,  7,  5,  3,  3,  2,  2 };
    }
}
