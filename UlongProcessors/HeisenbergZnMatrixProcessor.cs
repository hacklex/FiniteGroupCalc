using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace FiniteGroupCalc.UlongProcessors
{
    public class HeisenbergZnMatrixProcessor : UlongProcessorBase
    {
        public override ulong Identity => 0;
        public override ulong GetNext(ulong current) => current + 1;
        public override string CacheId => $"Heis_Z{Modulo}_{Order + 2}x{Order + 2}_{BasisType}";
        public override string DisplayName => $"Heisenberg(Z{Modulo.GetSubscript()}), {Order + 2}x{Order + 2}";
        public int BitsPerElement
        {
            get => _bitsPerElement; set
            {
                if (_bitsPerElement == value) return;
                _bitsPerElement = value;
                OnPropertyChanged(nameof(BitsPerElement));
                OnPropertyChanged(nameof(Modulo));
                OnPropertyChanged(nameof(MaxOrder));
                OnPropertyChanged(nameof(UlongCount));
                OnPropertyChanged(nameof(FreeElementCount));
            }
        }
        public override int Modulo => 1 << BitsPerElement;

        public ulong[] GetZorNoZBasis()
        {
            ulong[] result = new ulong[HasZInBasis() ? FreeElementCount : FreeElementCount - 1];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = 1ul << i * BitsPerElement;
            }
            return result;
        }

        public bool HasInversesInBasis() => BasisType == HeisenbergBasisKind.StandardWithInverses || BasisType == HeisenbergBasisKind.StandardWithZAndInverses;

        public override ulong[] GetStandardBasis()
        {
            var std = GetZorNoZBasis();
            return HasInversesInBasis() ? GetBasisWithInverses(std) : std;
        }

        public HeisenbergBasisKind BasisType
        {
            get => basisType; set
            {
                if (basisType == value) return;
                basisType = value;
                OnPropertyChanged(nameof(BasisType));
                OnPropertyChanged(nameof(UlongCount));
                OnPropertyChanged(nameof(FreeElementCount));
            }
        }
        public bool HasZInBasis() => BasisType == HeisenbergBasisKind.StandardWithZ || BasisType == HeisenbergBasisKind.StandardWithZAndInverses;
        public override ulong UlongCount => 1ul << FreeElementCount * BitsPerElement;
        public override string ToString() => "Fast Zₙ Heisenberg";

        private int _order = 1;
        private int _bitsPerElement = 1;
        private HeisenbergBasisKind basisType;

        public ulong ElementMask => (1ul << BitsPerElement) - 1;

        /// <summary>
        /// Defines the number of elements in the first row, excluding the first (fixed 1)
        /// </summary>
        [Description("Defines the number of elements in the first row, excluding the first (fixed 1)")]
        public override int Order
        {
            get => _order;
            set
            {
                if (_order == value) return;
                _order = value;
                OnPropertyChanged(nameof(Order));
                OnPropertyChanged(nameof(FreeElementCount));
            }
        }
        /// <summary>
        /// Defines the maximum possible order that would still fit in a ulong (Int64)
        /// </summary>
        [Description("Defines the maximum possible order that would still fit in a ulong (Int64)")]
        public override int MaxOrder => (64 / BitsPerElement - 1) / 2;

        [Description("Defines the total number of binary cells packed into a ulong (Int64)")]
        public override int FreeElementCount => Order * 2 + 1;

        public override int StandardBasisSize => Modulo == 2 ? BasisType switch
        {
            HeisenbergBasisKind.Standard => FreeElementCount - 1,
            HeisenbergBasisKind.StandardWithZ => FreeElementCount,
            HeisenbergBasisKind.StandardWithInverses => FreeElementCount - 1,
            HeisenbergBasisKind.StandardWithZAndInverses => FreeElementCount,
            _ => throw new NotImplementedException(),
        } : BasisType switch
        {
            HeisenbergBasisKind.Standard => FreeElementCount - 1,
            HeisenbergBasisKind.StandardWithZ => FreeElementCount,
            HeisenbergBasisKind.StandardWithInverses => (FreeElementCount - 1) * 2,
            HeisenbergBasisKind.StandardWithZAndInverses => FreeElementCount * 2,
            _ => throw new NotImplementedException(),
        };

        public override string StandardBasisDescription =>
            BasisType switch
            {
                HeisenbergBasisKind.Standard => $"Basis: {StandardBasisSize} elements, without Z)",
                HeisenbergBasisKind.StandardWithInverses => $"Basis: {StandardBasisSize} elements without Z, with inverses)",
                HeisenbergBasisKind.StandardWithZ => $"Basis: {StandardBasisSize} elements (with Z)",
                HeisenbergBasisKind.StandardWithZAndInverses => $"Basis: {StandardBasisSize} elements (with Z, with inverses)",
                _ => throw new ArgumentOutOfRangeException()
            };

        ulong Xi(ulong x, int i)
        {
            return x >> i * BitsPerElement & ElementMask;
        }
        ulong Yi(ulong x, int i)
        {
            return x >> (i + Order) * BitsPerElement & ElementMask;
        }
        ulong Z(ulong x)
        {
            return x >> 2 * Order * BitsPerElement & ElementMask;
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
                result |= (ulong)matrix[0, i + 1] << i * BitsPerElement;
                result |= (ulong)matrix[i + 1, Order + 1] << (Order + i) * BitsPerElement;
            }
            result |= (ulong)matrix[0, Order + 1] << 2 * Order * BitsPerElement;
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
            ulong result = 0;
            ulong xayb = 0;
            for (int i = 0; i < Order; i++)
            {
                var xa = Xi(a, i);
                var ya = Yi(a, i);
                var xb = Xi(b, i);
                var yb = Yi(b, i);
                result |= (xa + xb & ElementMask) << i * BitsPerElement;
                result |= (ya + yb & ElementMask) << (i + Order) * BitsPerElement;
                xayb += xa * yb;
            }
            xayb += Z(a) + Z(b);
            xayb &= ElementMask;
            result |= xayb << 2 * Order * BitsPerElement;
            return result;
        }
        public override string AsString(ulong packedValues)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i <= Order; i++)
            {
                sb.Append(new string(' ', (1 + BitsPerElement) * i));
                sb.Append("1" + new string(' ', BitsPerElement));
                for (int j = 0; j < Order - i; j++)
                {
                    sb.Append(GetElement(packedValues, i, j).ToString().PadLeft(BitsPerElement));
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
            for (int i = 0; i < Order; i++)
            {
                result |= ulong.Parse(firstLine[i + 1]) << i * BitsPerElement;
            }
            result |= ulong.Parse(firstLine[Order + 1]) << 2 * Order * BitsPerElement;
            for (int i = 0; i < Order; i++)
            {
                var line = lines[i + 1].Trim();
                var split = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (split.Length > 0 && ulong.TryParse(split.Last(), out ulong val))
                {
                    result |= val << (Order + i) * BitsPerElement;
                }
            }
            return result;
        }
    }
}
