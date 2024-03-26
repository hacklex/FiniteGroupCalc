using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using FiniteGroupCalc.PersistableCachers;

namespace FiniteGroupCalc
{
    public abstract class UlongProcessorBase : INotifyPropertyChanged 
    {
        public virtual bool StateIsKey => true;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public event PropertyChangedEventHandler? PropertyChanged;

        public virtual IComparer<ulong> GetUlongComparer() => Comparer<ulong>.Default;
        public abstract string CacheId { get; }
        public abstract string DisplayName { get; }
        public abstract int Modulo { get; }
        [Description("The amount of all valid Int64 values")]
        public abstract ulong UlongCount { get; }
        public abstract int MaxOrder { get; }
        public abstract int Order { get; set; }
        public abstract int FreeElementCount { get; } 
        public abstract ulong GetElement(ulong matrix, int row, int col);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract ulong Product(ulong a, ulong b);
        public abstract string AsString(ulong packedValues);
        public abstract ulong Parse(string input);
        public abstract ulong[] GetStandardBasis();
        public abstract int StandardBasisSize { get; }
        public abstract string StandardBasisDescription { get; }
        public abstract ulong Implode(byte[,] matrix);
        public abstract byte[,] Explode(ulong matrix);

        public abstract ulong Identity { get; }
        public abstract ulong GetNext(ulong current);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual ulong GetIth(int i) => (ulong)i;

        /// <summary>
        /// When calculating the diameter stats, skip generator sets that don't generate the full group
        /// </summary>
        public bool SkipNonFullGenerators { get; set; }
        /// <summary>
        /// How many elements to take as generator sets for the diameter statistics 
        /// </summary>
        [Description("How many elements to take as generator sets for the diameter statistics")]
        public int DiameterStatsBasisSize { get; set; } = 2;
        public bool AddInversesToDiameterStatsBases { get; set; } = false;
        public virtual ulong[] GetBasisWithInverses(ulong[] basis)
        { 
            var key = $"{CacheId}_inverses_[{basis.AsString()}]";
            if (PersistableUlongListCacher.Contains(key))
            {
                return PersistableUlongListCacher.Get(key);
            }

            var inverses = new ulong[basis.Length];
            for (ulong x = 1; x < UlongCount; x++)
            {
                for (int i = 0; i < basis.Length; i++)
                {
                    if (Product(x, basis[i]) == 0)
                    {
                        inverses[i] = x;
                    }
                }
            }
            var toStore = basis.Concat(inverses).Distinct().ToArray();
            PersistableUlongListCacher.Set(key, toStore);
            return toStore;
        }
        public string TestProduct()
        {            
            StringBuilder sb = new();
            for (ulong i = 0; i < UlongCount; i++)
            {
                if (Implode(Explode(i)) != i)
                {
                    sb.AppendLine($"Error Implode(Explode({i})) != {i}");
                    sb.AppendLine($"Explode =");
                    Explode(i).PrintTo(sb);
                    return sb.ToString();
                } 
                for (ulong j = 0; j < UlongCount; j++)
                {
                    if (Explode(Product(i, j)).NotEqual(Explode(i).Times(Explode(j), Modulo)))
                    {
                        sb.AppendLine($"Error, a={i}, b={j}");
                        sb.AppendLine($"Expected {Implode(Explode(i).Times(Explode(j), Modulo))}");
                        sb.AppendLine($"Got {(Product(i, j))}");
                        sb.AppendLine("A:");
                        if (this is TriangularZ2MatrixProcessor zt)
                        {
                            var rows = string.Join(" ", Enumerable.Range(0, Order + 1)
                                .Select(k => zt.GetUpperTriangularRow(j, k).ToBinString().PadLeft(Order + 1)));
                            var cols = string.Join(" ", Enumerable.Range(0, Order + 1)
                                .Select(k => zt.GetUpperTriangularCol(j, k).ToBinString().PadLeft(Order + 1)));
                            sb.AppendLine("Rows of B");
                            sb.AppendLine(rows);
                            sb.AppendLine("Cols of B");
                            sb.AppendLine(cols);
                        }
                        Explode(i).PrintTo(sb);
                        sb.AppendLine("B:");
                        Explode(j).PrintTo(sb);
                        sb.AppendLine("Expected:");
                        Explode(i).Times(Explode(j), Modulo).PrintTo(sb);
                        return sb.ToString();
                    }                    
                }
            }
            return "All tests green";
        }
    }
}
