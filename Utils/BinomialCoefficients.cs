using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiniteGroupCalc
{
    public static class BinomialCoefficients
    {
        public static readonly ulong[,] C = new ulong[300, 300];
        static BinomialCoefficients()
        {
            for (int k = 1; k < C.GetLength(1); k++) C[0, k] = 0;
            for (int n = 0; n < C.GetLength(0); n++) C[n, 0] = 1;
            for (int n = 1; n < C.GetLength(0); n++)
                for (int k = 1; k < C.GetLength(1); k++)
                    C[n, k] = C[n - 1, k - 1] + C[n - 1, k];
        }
    }
}
