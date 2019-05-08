using System;
using System.Collections.Generic;
using System.Text;

namespace PA6_Hamming
{
    class HammingCode
    {
        public static int[][] GetCodeGenerator()
        {
            int[][] temp = new int[7][];

            temp[0] = new int[4] { 1, 1, 0, 1 };
            temp[1] = new int[4] { 1, 0, 1, 1 };
            temp[2] = new int[4] { 1, 0, 0, 0 };
            temp[3] = new int[4] { 0, 1, 1, 1 };
            temp[4] = new int[4] { 0, 1, 0, 0 };
            temp[5] = new int[4] { 0, 0, 1, 0 };
            temp[6] = new int[4] { 0, 0, 0, 1 };

            return temp;
        }
    }
}
