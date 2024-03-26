using System.Runtime.CompilerServices;
using System.Text;

namespace FiniteGroupCalc
{
    public static class Extensions
    {
        /// <summary>Prints a set of space-padded rows of values, line by line, to a <see cref="StringBuilder"/>.</summary>
        /// <param name="stringBuilder">The <see cref="StringBuilder"/> to print to.</param>
        /// <param name="rowNames">Row names to print in the first column</param>
        /// <param name="rows">Rows data</param>
        public static void PrintMultipleRows(this StringBuilder stringBuilder, string[] rowNames, params int[][] rows)
        {
            if (rows == null) return;
            var maxRowLength = rows.Max(x => x.Length);
            var maxColWidths = new int[maxRowLength];
            var maxRowNameLength = rowNames.Max(x => x.Length) + 1;

            for (int i = 0; i < maxRowLength; i++)            
                maxColWidths[i] = rows.Max(x => (i < x.Length ? x[i] : 0).ToString().Length + 1);
            
            for (int i = 0; i < rows.Length; i++)
            {
                stringBuilder.Append(rowNames[i].PadLeft(maxRowNameLength));
                for (int j = 0; j < rows[i].Length; j++)
                {
                    if (rows.Length <= i) stringBuilder.Append(0.ToString().PadLeft(maxColWidths[j]));
                    else if (rows[i].Length <= j) stringBuilder.Append(0.ToString().PadLeft(maxColWidths[j]));
                    else stringBuilder.Append(rows[i][j].ToString().PadLeft(maxColWidths[j]));
                }
                stringBuilder.AppendLine();
            }
        }
        /// <summary>Prints a space-padded row of values to a <see cref="StringBuilder"/>.</summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to print to.</param>
        /// <param name="values">A row of values to print</param>
        public static void PrintRow(this StringBuilder sb, int[] values, int? cellSizeInCharacters = null)
        {
            if (values == null || sb == null) return;
            var maxLen = cellSizeInCharacters ?? values.Max().ToString().Length + 1;
            for (int i = 0; i < values.Length; i++)            
                sb.Append(values[i].ToString()?.PadLeft(maxLen));            
            sb.AppendLine();
        }

        /// <summary>Converts an array of integers to a string.</summary>
        /// <param name="ints">The array of integers to convert.</param>
        /// <returns> A string representation of the array.</returns>
        public static string AsString(this int[] ints)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < ints.Length; i++)
            {
                sb.Append(ints[i]);
                if (i < ints.Length - 1)
                {
                    sb.Append(", ");
                }
            }
            return sb.ToString();
        }
        /// <summary>Converts a string to an array of integers.</summary>
        public static int[] AsIntArray(this string str)
        {
            var parts = str.Split(',');
            var result = new int[parts.Length];
            for (int i = 0; i < parts.Length; i++)
            {
                result[i] = int.Parse(parts[i]);
            }
            return result;
        }
        /// <summary>Converts an array of unsigned long integers to a string.</summary>
        /// <param name="items">The array to convert</param>
        /// <returns>A comma-separated string representation of the array</returns>
        public static string AsString(this ulong[] items)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < items.Length; i++)
            {
                sb.Append(items[i]);
                if (i < items.Length - 1)
                {
                    sb.Append(", ");
                }
            }
            return $"[{sb}]";
        }
        /// <summary>Checks if a byte has an odd number of bits set.</summary>
        /// <param name="value">The byte value to check</param>
        /// <returns><c>true</c> if the byte has an odd number of bits set; otherwise, <c>false</c>.</returns>
        static bool IsParityOdd(this byte value)
        {
            int parity = 0;
            while(value != 0)
            {
                parity ^= value & 1;
                value >>= 1;
            }
            return parity != 0;
        }
        /// <summary>A lookup table for the parity of a byte.</summary>
        static byte[] ByteParityLookup = Enumerable.Range(0, 256)
            .Select(i => IsParityOdd((byte)i) ? (byte)1 : (byte)0).ToArray();

        /// <summary>Gets the parity of a 64-bit unsigned integer by XORing the parity of its bytes. </summary>
        /// <param name="value">The 64-bit unsigned integer to get the parity of.</param>
        /// <returns>The parity bit of the 64-bit unsigned integer, as a 64-bit unsigned integer.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetParity(this ulong value)
        {
            var u32 = (uint)(value >> 32) ^ (uint)(value & 0xFFFFFFFF);
            var u16 = (ushort)(u32 >> 16) ^ (ushort)(u32 & 0xFFFF);
            var u8 = (byte)(u16 >> 8) ^ (byte)(u16 & 0xFF);
            return ByteParityLookup[u8];
        }
        /// <summary>Compares two two-dimensional byte arrays for equality.</summary>
        /// <param name="a">The first two-dimensional byte array to compare.</param>
        /// <param name="b">The second two-dimensional byte array to compare.</param>
        /// <returns><c>true</c> if the two arrays are not equal; otherwise, <c>false</c>.</returns>
        public static bool NotEqual(this byte[,] a, byte[,] b)
        {
            if (a.GetLength(0) != b.GetLength(0) || a.GetLength(1) != b.GetLength(1)) return true;
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < a.GetLength(1); j++)
                {
                    if (a[i, j] != b[i, j]) return true;
                }
            }
            return false;
        }
        /// <summary>Prints a two-dimensional byte array to a string builder.</summary>
        /// <param name="a">The array to print</param>
        /// <param name="sb">The string builder to print the array to</param>
        /// <param name="space">The separator string to use</param>
        public static void PrintTo(this byte[,] a, StringBuilder sb, string space = " ")
        {
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < a.GetLength(1); j++)
                {
                    sb.Append(a[i, j]);
                    if (j < a.GetLength(1) - 1)
                    {
                        sb.Append(space);
                    }
                }
                sb.AppendLine();
            }
        }
        /// <summary>Converts a 64-bit unsigned integer to a binary string.</summary>
        /// <param name="u">The integer to convert</param>
        /// <returns>A string of 0s and 1s representing the input integer</returns>
        public static string ToBinString(this ulong u)
        {
            return Convert.ToString((long)u, 2);
        }
        /// <summary>Converts an integer to a binary string using subscript characters</summary>
        /// <param name="i">The integer to convert</param>
        /// <returns>The string representation of the input number composed from subscript unicode characters</returns>
        public static string GetSubscript(this int i)
        {
            var charArray = i.ToString().Trim().ToCharArray();
            for (int j = 0; j < charArray.Length; j++)            
                charArray[j] = charArray[j] == '-' ? '₋' : "₀₁₂₃₄₅₆₇₈₉"[charArray[j] - '0'];            
            return $"{new string(charArray)}";
        }
        /// <summary>
        /// Multiplies two square matrices of bytes modulo a given number with no performance optimizations.
        /// </summary>
        /// <param name="a">The left-hand side matrix of the product</param>
        /// <param name="b">The right-hand side matrix of the product</param>
        /// <param name="modulo">The modulo</param>
        /// <remarks>This method does not utilize any optimizations, and therefore takes O(n³) operations</remarks>
        /// <returns>The matrix product array</returns>
        public static byte[,] Times(this byte[,] a, byte[,] b, int modulo)
        {
            if (a.GetLength(1) != b.GetLength(0)) throw new ArgumentException("Matrix dimensions do not match");
            var firstSize = a.GetLength(0);
            var strideSize = a.GetLength(1);
            var secondSize = b.GetLength(1);            
            var result = new byte[firstSize, secondSize];
            for (int i = 0; i < firstSize; i++)
            {
                for (int j = 0; j < secondSize; j++)
                {
                    for (int k = 0; k < strideSize; k++)
                    {
                        result[i, j] += (byte)(a[i, k] * b[k, j]);
                        result[i, j] %= (byte)modulo;
                    }
                }
            }
            return result;
        }
    }
}
