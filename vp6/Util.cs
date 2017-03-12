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

        public static byte[] GetSlice(byte[,,,] arr, int x, int y,int z)
        {
            int size = arr.GetLength(3);
            byte[] sub = new byte[size];

            for (int i = 0; i < size; ++i)
            {
                sub[i] = arr[x, y,z, i];
            }

            return sub;
        }

        public static byte[] Transpose(byte[] arr)
        {
            byte[] result = new byte[arr.Length];

            for(int i=0;i<arr.Length;++i)
            {
                byte x = arr[i];
                result[i] = (byte)(((x) >> 3) | (((x) & 7) << 3));
            }

            return result;
        }
    }
}
