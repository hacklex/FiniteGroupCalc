using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace FiniteGroupCalc
{
    public class TriangularZnMatrixProcessor : UlongProcessorBase
    {
        public override string CacheId => $"Tri_Z{Modulo}_{Order + 1}x{Order + 1}_{BasisType}";
        public override string DisplayName => $"Triangular(Z{Modulo.GetSubscript()}), {Order + 1}x{Order + 1}";
        public TriangularBasisKind BasisType { get; set; } = TriangularBasisKind.Standard;
        public override int Modulo => 1 << BitsPerElement;
        public override byte[,] Explode(ulong matrix)
        {
            byte[,] bytes = new byte[Order + 1, Order + 1];
            for (int i = 0; i < Order + 1; i++)
            {
                bytes[i, i] = 1; 
                for (int j = 0; j < Order - i; j++)
                {
                    bytes[i, i + j + 1] = (byte)GetElement(matrix, i, i + j + 1);
                }
            } 
            return bytes;
        }
        public override ulong Implode(byte[,] matrix)
        {
            ulong result = 0;
            int offset = 0;
            for (int i = 0; i <= Order; i++)
            {
                for (int j = i + 1; j <= Order; j++)
                {
                    result |= (ulong)matrix[i, j] << offset;
                    offset += BitsPerElement;
                }
            }
            return result;
        }
        public override int StandardBasisSize
        {
            get
            {
                if (Modulo == 2) return BasisType switch
                {
                    TriangularBasisKind.Standard => FreeElementCount,
                    TriangularBasisKind.Ribbon => Order,
                    TriangularBasisKind.StandardAndInverses => FreeElementCount,
                    TriangularBasisKind.RibbonAndInverses => Order,
                    _ => throw new ArgumentOutOfRangeException()
                };
                else return BasisType switch
                {
                    TriangularBasisKind.Standard => FreeElementCount,
                    TriangularBasisKind.Ribbon => Order,
                    TriangularBasisKind.StandardAndInverses => FreeElementCount * 2,
                    TriangularBasisKind.RibbonAndInverses => Order * 2,                
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        public override string StandardBasisDescription =>
            BasisType switch
            {
                TriangularBasisKind.Standard => $"Basis: standard, no inverses ({FreeElementCount})",
                TriangularBasisKind.Ribbon => $"Basis: ribbon, no inverses ({Order})",
                TriangularBasisKind.StandardAndInverses => $"Basis: standard with inverses ({GetStandardBasis().Length})",
                TriangularBasisKind.RibbonAndInverses => $"Basis: ribbon with inverses ({GetStandardBasis().Length})",
                _ => throw new ArgumentOutOfRangeException()
            };

        public override ulong[] GetStandardBasis() =>
            BasisType switch
            {
                TriangularBasisKind.Standard => GetFullBasis(),
                TriangularBasisKind.Ribbon => GetUpperRibbonBasis(),
                TriangularBasisKind.StandardAndInverses => GetBasisWithInverses(GetFullBasis()),
                TriangularBasisKind.RibbonAndInverses => GetBasisWithInverses(GetUpperRibbonBasis()),
            };

        public ulong[] GetUpperRibbonBasis()
        {
            ulong[] result = new ulong[Order];
            byte[,] exploded = Explode(0);
            for (int i = 0; i < Order; i++)
            {
                exploded[i, i + 1] = 1;
                result[i] = Implode(exploded);
                exploded[i, i + 1] = 0;
            }
            return result;
        }

        public ulong[] GetFullBasis()
        {
            ulong[] result = new ulong[FreeElementCount];
            for (int i = 0; i < FreeElementCount; i++)
            {
                result[i] = 1ul << (i * BitsPerElement);
            }
            return result;
        }
        public override string ToString() => $"Zₙ Triangular";
        private int _order = 1;
        private int _bitsPerElement = 1;
        public override ulong UlongCount => 1ul << (BitsPerElement * FreeElementCount);
        

        [Description("Defines the number of elements in the first row, excluding the first (fixed 1)")]
        public override int Order
        {
            get => _order; 
            set
            {
                if (_order == value) return;
                if (value > MaxOrder) value = MaxOrder;
                _order = value;
                OnPropertyChanged(nameof(Order));
            }
        }
        [Description("Defines the number of bits per element")]
        public int BitsPerElement 
        { 
            get => _bitsPerElement; 
            set
            {
                if (value < 1 || value > 64) value = Math.Clamp(value, 1, 64);
                if (_bitsPerElement == value) return;
                _bitsPerElement = value;
                OnPropertyChanged(nameof(BitsPerElement));                
                if (Order > MaxOrder)
                    Order = MaxOrder;
                OnPropertyChanged(nameof(Order));
                OnPropertyChanged(nameof(MaxOrder));
                OnPropertyChanged(nameof(FreeElementCount)); 
            }
        }
        [Description("Defines the maximum possible order that would still fit in a ulong (Int64)")]
        public override int MaxOrder 
        { 
            get
            {
                int totalCount = BitsPerElement;
                int order = 1;
                int curRowSize = 1;
                while (totalCount <= 64)
                {
                    order++;
                    curRowSize++;
                    totalCount += BitsPerElement * curRowSize;
                }
                return order - 1;
            } 
        }
        [Description("Defines the total number of binary cells packed into a ulong (Int64)")]
        public override int FreeElementCount => Order * (Order + 1) / 2;

        [Description("Defines the bit mask to extract the first element from a ulong (Int64)")]
        public ulong NumberMask => (1ul << BitsPerElement) - 1;

        public override ulong Identity => 0;
        public override ulong GetNext(ulong current) => current + 1;

        int GetOffset(int row, int col)
        {
            return (row * Order - (row * (row - 1) / 2) + col) * (BitsPerElement);
        } 
        public ulong ReplaceElement(ulong matrix, int row, int col, ulong element)
        {
            var offset = GetOffset(row, col);
            var mask = ~(NumberMask << offset);
            return (matrix & mask) | (element << offset);

        }
        public override ulong GetElement(ulong matrix, int row, int col)
        {
            if (row == col) return 1ul;
            if (row > col) return 0ul;
            col -= row + 1;
            var offset = GetOffset(row, col);
            return (matrix >> offset) & NumberMask;
        }

        public override ulong Product(ulong a, ulong b)
        {
            ulong result = 0;
            for (var i = 0; i < Order; i++)
            {
                for(var j = 0; j < Order - i; j++)
                {
                    var row = i;
                    var col = j + i + 1;
                    result = ReplaceElement(result, i, j, DotProduct(a, b, row, col));
                }
            }
            return result;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong DotProduct(ulong a, ulong b, int aRow, int bCol)
        { 
            ulong result = 0;
             
            for (int i = 0; i <= Order; i++)
            {
                var aElement = GetElement(a, aRow, i); 
                var bElement = GetElement(b, i, bCol);
                result += aElement * bElement;
            }
             
            return result & NumberMask;
        }
        public override string AsString(ulong packedValues)
        {
            int maxDecimalDigits = (int)Math.Ceiling(Math.Log10(1ul << BitsPerElement));

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i <= Order; i++)
            {
                sb.Append(new string(' ', (1 + maxDecimalDigits) * i));
                sb.Append("1".PadLeft(maxDecimalDigits + 1));
                for (int j = 0; j < Order - i; j++)
                {
                    sb.Append(GetElement(packedValues, i, j).ToString().PadLeft(maxDecimalDigits));
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
            for (int i = 0; i < Order; i++)
            {
                var line = lines[i].Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < Order - i; j++)
                {
                    result |= (NumberMask & ulong.Parse( line[j+1])) << GetOffset(i, j);
                }
            }
            return result;
        }

    }
}
