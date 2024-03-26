using System.Runtime.CompilerServices;

namespace FiniteGroupCalc
{
    public class PermutationProcessor : UlongProcessorBase
    {
        public override string CacheId => $"Permuter_{Order}";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override ulong GetIth(int i) => GetIthPermutationOfOrder((ulong)i, (ulong)Order);

        public override string DisplayName => $"Permuter [{Order}]";
        
        public override int Modulo => 16;

        public override ulong UlongCount => Factorial(Order);

        public override int MaxOrder => 11;

        public override int Order { get; set; }

        static ulong[] Factorials = Enumerable.Range(0, 13).Select(FactorialInternal).ToArray();
        private static ulong FactorialInternal(int n)
        {
            ulong result = 1;
            for (uint i = 1; i <= n; i++)
            {
                result *= i;
            }
            return result;
        }

        public ulong ApplyPermutation(ulong perm, ulong x)
        {
            ulong result = 0;
            for (int i = 0; i < Order; i++)
            {
                int xDigitShift = (int)(perm & 0xf) * 4;
                perm >>= 4;
                int resultShift = i * 4;
                ulong digit = (x >> xDigitShift) & 0xf;
                result |= (digit << resultShift);
            }
            return result;
        }

        public static ulong Factorial(int n) => Factorials[n];
        
        public override int FreeElementCount => Order - 1;

        public override int StandardBasisSize => FreeElementCount;

        public override string StandardBasisDescription => "(N-1) swaps";

        public override string AsString(ulong packedValues) => $"{packedValues:X}";


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ulong GetDigit(ulong n, int i) => (n >> i * 4) & 0xF;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ulong SwapDigits(ulong n, int i, int j)
        {
            ulong mask = 0xFul << i * 4 | 0xFul << j * 4;
            ulong a = GetDigit(n, i);
            ulong b = GetDigit(n, j);
            n &= ~mask;
            n |= a << j * 4;
            n |= b << i * 4;
            return n;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong NextPermutationUlong(ulong cur)
        {
            if (Order <= 1) return cur;
            int i = Order - 2;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            ulong ReverseDigits(ulong a, int i, int j)
            {
                while (i < j)
                    a = SwapDigits(a, i++, j--);
                return a;
            }

            while (i >= 0 && GetDigit(cur, i) >= GetDigit(cur, i + 1)) i--;
            if (i >= 0)
            {
                int j = Order - 1;
                while (GetDigit(cur, j) <= GetDigit(cur, i)) j--;

                cur = SwapDigits(cur, i, j);
            }
            return ReverseDigits(cur, i + 1, Order - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong PermIndex(ulong perm)
        {
            ulong res = 0;
            for (int i = 0; i < Order; i++)
            {
                int count = 0;
                for (int j = i + 1; j < Order; j++)
                {
                    if (GetDigit(perm,(ulong)j) < GetDigit(perm, (ulong)i)) count++;
                }
                res += (ulong)count * Factorial(Order - i - 1);
            }
            return res;
        }
        static int ReverseDigitOrderUlongCompare(ulong a, ulong b)
        {
            for (int i = 0; i < 16; i++)
            {
                var aDigit = (byte)((a >> (i * 4)) & 0xf);
                var bDigit = (byte)((b >> (i * 4)) & 0xf);
                if (aDigit < bDigit) return -1;
                if (aDigit > bDigit) return 1;
            }
            return 0;
        }
        public override bool StateIsKey => false;
        class RevUlongComparer : IComparer<ulong>
        {
            public int Compare(ulong x, ulong y) => ReverseDigitOrderUlongCompare(x, y);
        }
        public override IComparer<ulong> GetUlongComparer() => new RevUlongComparer();

        public static ulong RemoveDigit(ulong x, ulong digit)
        {
            if (digit == 0) return x >> 4; 
            var lowerMask = (1ul << (int)digit * 4) - 1;
            var higherMask = (~lowerMask) << 4;
            var onlyHigher = x & higherMask;
            return (onlyHigher >> 4) | (x & lowerMask);
        }

        public static ulong GetDigit(ulong x, ulong pos) => (x >> (int)(pos * 4)) & 0xF;

        public static ulong GetIthPermutationOfOrder(ulong x, ulong order)
        {
            var proc = new PermutationProcessor() { Order = (int)order };
            var remain = proc.Identity;
             
            ulong result = 0;
            for (ulong i = order; i > 0; i--)
            {
                var curFactorial = Factorial((int)(i - 1));
                var pos = x / curFactorial;
                result |= GetDigit(remain, pos) << (int)(order - i) * 4;
                remain = RemoveDigit(remain, pos);
                x %= curFactorial;
            }
            return result;
        }

        static ulong GetIdentity(int n)
        {
            ulong result = 0;
            for (var order = 0; order < n; order++)
                result |= (ulong)order << (4 * order);
            return result;
        }

        public override ulong Identity => GetIdentity(Order);
        
        public override ulong GetNext(ulong current) => NextPermutationUlong(current);

        public override byte[,] Explode(ulong matrix)
        {
            throw new NotImplementedException();
        }

        public override ulong GetElement(ulong matrix, int row, int col)
        {
            throw new NotImplementedException();
        }

        public override ulong[] GetStandardBasis()
        {
            var result = new ulong[Order-1];
            for (int i = 1; i < Order; i++)
            {
                var perm = SwapDigits(Identity, i, i - 1);
                result[i-1] = perm;
            }
            return result;
        }       

        public override ulong Implode(byte[,] matrix)
        {
            throw new NotImplementedException();
        }

        public override ulong Parse(string input)
        {
            throw new NotImplementedException();
        }

        public override ulong Product(ulong a, ulong b) => ApplyPermutation(b, a);
        public override string ToString() => "Permuter";
    }
}
