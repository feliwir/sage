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

        public static byte[] GetSlice(byte[,] arr,int x)
        {
            int size = arr.GetLength(1);
            byte[] sub = new byte[size];
           
            for(int i=0;i<size;++i)
            {
                sub[i] = arr[x,i];
            }

            return sub;
        }

        public static byte[] GetSlice(byte[,,] arr, int x,int y)
        {
            int size = arr.GetLength(2);
            byte[] sub = new byte[size];

            for (int i = 0; i < size; ++i)
            {
                sub[i] = arr[x,y, i];
            }

            return sub;
        }
    }
}
