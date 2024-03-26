using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace FiniteGroupCalc
{
    public enum HeisenbergBasisKind
    {
        Standard,
        StandardWithZ,
        StandardWithInverses,
        StandardWithZAndInverses,
    }
    public enum HeisenbergZ2BasisKind
    {
        Standard,
        StandardWithZ, 
    }

    public class HeisenbergZ2MatrixProcessor : UlongProcessorBase
    {
        public override ulong Identity => 0;
        public override ulong GetNext(ulong current) => current + 1;
        public override string CacheId => $"Heis_Z{Modulo}_{Order + 2}x{Order + 2}_{BasisType}";
        public override string DisplayName => $"Heisenberg(Z₂), {Order + 2}x{Order + 2}";

        public override int Modulo => 2;
        public HeisenbergZ2BasisKind BasisType
        {
            get => basisType; set
            {
                if (basisType == value) return;
                basisType = value;
                OnPropertyChanged(nameof(BasisType));
                OnPropertyChanged(nameof(FreeElementCount));
            }
        }
        public override int StandardBasisSize => BasisType switch
        {
            HeisenbergZ2BasisKind.Standard => FreeElementCount - 1,
            HeisenbergZ2BasisKind.StandardWithZ => FreeElementCount, 
            _ => throw new ArgumentOutOfRangeException()
        };

        public override string StandardBasisDescription =>
            BasisType switch
            {
                HeisenbergZ2BasisKind.Standard => $"Basis: {FreeElementCount - 1} elements, no Z",
                HeisenbergZ2BasisKind.StandardWithZ => $"Basis: {FreeElementCount} elements, with Z", 
                _ => throw new System.ArgumentOutOfRangeException()
            };
        public override ulong[] GetStandardBasis()
        {
            var size = FreeElementCount - (BasisType == HeisenbergZ2BasisKind.Standard ? 1 : 0);
            ulong[] result = new ulong[size];
            for (int i = 0; i < size; i++)
            {
                result[i] = 1ul << i;
            }
            return result;
        }
        public override ulong UlongCount => 1ul << (Order * 2 + 1);
        public override string ToString() => "Fast Z₂ Heisenberg";

        private int _order = 1;
        private HeisenbergZ2BasisKind basisType = HeisenbergZ2BasisKind.StandardWithZ;

        [Description("Defines the number of elements in the first row, excluding the first (fixed 1)")]
        public override int Order
        {
            get => _order;
            set
            {
                if (_order == value) return;
                _order = value;
                OnPropertyChanged(nameof(Order));
            }
        }
        [Description("Defines the maximum possible order that would still fit in a ulong (Int64)")]
        public override int MaxOrder => 14;
        [Description("Defines the total number of binary cells packed into a ulong (Int64)")]
        public override int FreeElementCount => Order * 2 + 1;

        ulong Xi(ulong x, int i)
        {
            return ((x >> i) & 1);
        }
        ulong Yi(ulong x, int i)
        {
            return ((x >> (i + Order)) & 1);
        }
        ulong Z(ulong x)
        {
            return ((x >> (2 * Order)) & 1);
        }
        public override byte[,] Explode(ulong matrix)
        {
            byte[,] bytes = new byte[Order + 2, Order + 2];
            for (int i = 0; i <= Order + 1; i++)
            {
                bytes[i, i] = 1;
                if (i < Order)
                {
                    bytes[0, i + 1] = (byte)Xi(matrix, i);
                    bytes[i + 1, Order + 1] = (byte)Yi(matrix, i);
                }
            }
            bytes[0, Order + 1] = (byte)Z(matrix);
            return bytes;
        }
        public override ulong Implode(byte[,] matrix)
        {
            ulong result = 0;
            for (int i = 0; i < Order; i++)
            {
                result |= (ulong)matrix[0, i + 1] << i;
                result |= (ulong)matrix[i + 1, Order + 1] << (Order + i);
            }
            result |= (ulong)matrix[0, Order + 1] << (2 * Order);
            return result;
        }
        public override ulong GetElement(ulong matrix, int row, int col)
        {
            if (row == col) return 1;
            if (col == Order && row == 0) return Z(matrix);
            if (col == Order) return Yi(matrix, row);
            if (row < col) return 0;
            if (row == 0) return Xi(matrix, col - 1);
            return 0;
        }
       
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override ulong Product(ulong a, ulong b)
        {
            var xored = a ^ b;
             
            var rowBits = Z(a) << Order + 1;
            rowBits |= ((a & ((1ul << Order) - 1)) << 1);
            rowBits |= 1;

            var colBits = 1ul << Order + 1;
            colBits |= ((b >> Order) & ((1ul << Order) - 1)) << 1;
            colBits |= Z(b);
            
            var dot = rowBits & colBits;

            var parity = dot.GetParity();

            xored &= ~((1ul << 2*Order));
            xored |= (parity << 2*Order);

            //var zMask = (1UL << (2 * Order));

            //var xDotY = a & (b << Order); // in place of y lies the dot product
            //for (int i = 0; i < Order; i++)
            //{
            //    xDotY <<= 1;
            //    xored ^= (xDotY & zMask);
            //}
            return xored; 
        }
        public override string AsString(ulong packedValues)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i <= Order; i++)
            {
                sb.Append(new string(' ', 2 * i));
                sb.Append("1 ");
                for (int j = 0; j < Order - i; j++)
                {
                    sb.Append(GetElement(packedValues, i, j));
                    if (j < Order - i - 1)
                        sb.Append(" ");
                }
                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }
        public override ulong Parse(string input)
        {
            ulong result = 0;
            var lines = Regex.Split(input, Environment.NewLine);
            if (lines.Length < Order) return 0;
            var firstLine = lines[0].Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (firstLine.Length < Order + 2) return 0;
            for (int i = 0; i<Order; i++)
            {
                result |= (ulong.Parse(firstLine[i + 1]) << i);
            }
            result |= (ulong.Parse(firstLine[Order + 1]) << (2 * Order));
            for (int i = 1; i < Order; i++)
            {
                var line = lines[i].Trim();
                if (line.EndsWith("1"))
                {
                    result |= 1UL << (Order + i);
                } 
            } 
            return result;
        }

    }
}
