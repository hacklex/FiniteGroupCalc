using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace FiniteGroupCalc
{
    public enum TriangularBasisKind
    {
        Standard,
        Ribbon,
        StandardAndInverses,
        RibbonAndInverses,
    }
    public class TriangularZ2MatrixProcessor : UlongProcessorBase
    {
        public override ulong Identity => 0;
        public override ulong GetNext(ulong current) => current + 1;
        public override int StandardBasisSize => BasisType switch
        {
            TriangularBasisKind.Standard => FreeElementCount,
            TriangularBasisKind.Ribbon => Order,
            TriangularBasisKind.StandardAndInverses => FreeElementCount,
            TriangularBasisKind.RibbonAndInverses => Order,
            _ => throw new ArgumentOutOfRangeException()
        };

        public override string CacheId => $"Tri_Z2_{Order + 1}x{Order + 1}_{BasisType}";
        public override string DisplayName => $"Triangular(Z₂), {Order+1}x{Order+1}";
        public override byte[,] Explode(ulong matrix)
        {
            byte[,] bytes = new byte[Order + 1, Order + 1];
            for (int i = 0; i < Order + 1; i++)
            {
                bytes[i, i] = 1;
                for (int j = 0; j < Order - i; j++)
                {
                    bytes[i, i + j + 1] = (byte)GetElement(matrix, i, j);
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
                    offset++;
                }
            }
            return result;
        }

        public override int Modulo => 2;
        public TriangularBasisKind BasisType { get; set; }

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

        public override string StandardBasisDescription =>
            BasisType switch {
                TriangularBasisKind.Standard => $"Basis: {FreeElementCount} elements (each free 1)",
                TriangularBasisKind.Ribbon => $"Basis: Upper Ribbon, ({Order} 1s) above main diagonal",
                TriangularBasisKind.StandardAndInverses => $"Basis: Standard and inverses (total {FreeElementCount} elements)",
                TriangularBasisKind.RibbonAndInverses => $"Basis: Ribbon and Inverses (total {Order*2} elements)",
                _ => throw new ArgumentOutOfRangeException()                    
            };
        public ulong[] GetFullBasis()
        {
            ulong[] result = new ulong[FreeElementCount];
            for (int i = 0; i < FreeElementCount; i++)
            {
                result[i] = 1ul << i;
            }
            return result;
        }
        public override ulong UlongCount => 1ul << FreeElementCount;
        public override string ToString() => "Fast Z₂ Triangular";

        private int _order = 1;

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
            }
        }
        /// <summary>
        /// Defines the maximum possible order that would still fit in a ulong (Int64)
        /// </summary>
        [Description("Defines the maximum possible order that would still fit in a ulong (Int64)")]
        public override int MaxOrder => 10;
        [Description("Defines the total number of binary cells packed into a ulong (Int64)")]
        public override int FreeElementCount => Order * (Order + 1) / 2;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int GetUpperTriangularZ2RowOffset(int row)
        {
            return row * Order - (row * (row - 1) / 2);
        }        
        public ulong GetUpperTriangularRow(ulong packedValues, int row)
        {
            var rowPadding = (1UL << row);
            var rowLenInBits = Order - row;
            var rowOffset = GetUpperTriangularZ2RowOffset(row);
            var rowContent = (packedValues >> rowOffset) & ((1UL << rowLenInBits) - 1);            
            return (rowContent << (row + 1)) | rowPadding;
        }
        public ulong GetUpperTriangularCol(ulong packedValues, int col)
        {
            col--;
            int nDeltaSimplified(int row) => Order - row - 1;
            ulong result = 0;
            int offset = col;
            int vecLength = col + 1;
            for (int i = 0; i < vecLength; i++)
            {
                result |= ((packedValues >> offset) & 1) << i;
                offset += nDeltaSimplified(i);
            }
            result |= 1ul << vecLength;
            return result;
        }

        public override ulong GetElement(ulong matrix, int row, int col)
        {
            return (matrix >> (GetUpperTriangularZ2RowOffset(row) + col)) & 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override ulong Product(ulong a, ulong b)
        {
            ulong result = 0;
            for (int row = 0; row < Order; row++)
            {
                for (int col = 0; col < Order - row; col++)
                { 
                    var aRow = GetUpperTriangularRow(a, row);

                    var bCol = GetUpperTriangularCol(b, col+1+row);


                    var dotProduct = (aRow & bCol).GetParity();
                    var offset = GetUpperTriangularZ2RowOffset(row) + col;
                    result |= (dotProduct << offset);
                }
            }
            return result;
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
            for (int i = 0; i < Order; i++)
            {
                var line = lines[i].Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < Order - i; j++)
                {
                    if (line[j + 1] == "1")
                    {
                        result |= 1UL << (GetUpperTriangularZ2RowOffset(i) + j);
                    }
                }
            }
            return result;
        }
    }
}
