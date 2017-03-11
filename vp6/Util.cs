using System;
using System.Collections.Generic;
using System.Text;

namespace sage.vp6
{
    class Util
    {
        public static int Clip(int input,int min,int max)
        {
            if(input>max)
            {
                return max;
            }
            else if(input<min)
            {
                return min;
            }
            else
            {
                return input;
            }
        }
    }
}
