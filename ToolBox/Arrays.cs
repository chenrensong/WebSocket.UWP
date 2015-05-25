﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyWebSocket.ToolBox
{
    public class Arrays
    {
        public static int binarySearch(int[] a, int key)
        {
            return binarySearch0(a, 0, a.Length, key);
        }

        public static byte[] copyOfRange(byte[] original, int from, int to)
        {
            int newLength = to - from;
            if (newLength < 0)
                throw new Exception(from + " > " + to);
            byte[] copy = new byte[newLength];
            Array.Copy(original, from, copy, 0,
                             Math.Min(original.Length - from, newLength));
            return copy;
        }


        private static int binarySearch0(int[] a, int fromIndex, int toIndex, int key)
        {
            int low = fromIndex;
            int high = toIndex - 1;

            while (low <= high)
            {

                int mid = (low + high) >> 1;//>>>
                int midVal = a[mid];

                if (midVal < key)
                    low = mid + 1;
                else if (midVal > key)
                    high = mid - 1;
                else
                    return mid; // key found
            }
            return -(low + 1);  // key not found.
        }

    }
}
