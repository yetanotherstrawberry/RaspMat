using System;
using System.Threading.Tasks;

namespace RaspMat.Helpers
{
    /// <summary>
    /// Helpers for 2D jagged arrays.
    /// </summary>
    internal static class JaggedArrayHelper
    {

        /// <summary>
        /// Copies a 2D jagged array.
        /// </summary>
        /// <typeparam name="T">Input array type.</typeparam>
        /// <param name="input">Array to be copied.</param>
        /// <param name="startCol">Column (2nd dimension) index to start from.</param>
        /// <param name="colLength">Designated length of the 2nd dimension. Will copy everything past starting index if <see langword="null"/>.</param>
        /// <param name="startRow">Row (1st dimension) index to start from.</param>
        /// <param name="rowLength">Designated length of the 1st dimension. Will copy everything past starting index if <see langword="null"/>.</param>
        /// <returns>New array with elements copied from input.</returns>
        public static T[][] Copy<T>(this T[][] input, int startCol = 0, int? colLength = null, int startRow = 0, int? rowLength = null)
        {
            var ret = new T[rowLength ?? input.Length][];

            Parallel.For(startRow, ret.Length, row =>
            {
                ret[row] = new T[colLength ?? (input[row].Length - startCol)];
                Array.Copy(input[row], startCol, ret[row], 0, ret[row].Length);
            });

            return ret;
        }

        /// <summary>
        /// Converts a 2D jagged array.
        /// </summary>
        /// <typeparam name="T">Input array type.</typeparam>
        /// <typeparam name="Y">Type of the returned array.</typeparam>
        /// <param name="input">Array to be converted. Will not be modified.</param>
        /// <param name="converter">Converter changing the type for a single element of the input array.</param>
        /// <returns>New array resulted from converting the input.</returns>
        public static Y[][] Convert<T, Y>(this T[][] input, Converter<T, Y> converter)
        {
            var ret = new Y[input.Length][];

            Parallel.For(0, ret.Length, row =>
            {
                ret[row] = Array.ConvertAll(input[row], converter);
            });

            return ret;
        }

        /// <summary>
        /// Creates a 2D array.
        /// </summary>
        /// <typeparam name="T">Type of the array.</typeparam>
        /// <param name="rows">Length of the 1st dimension.</param>
        /// <param name="cols">Length od the 2nd dimension.</param>
        /// <param name="creator">Function that accepts 2 indexes and returns value to be assigned.</param>
        /// <returns>2D array of <c>T</c> with values from <c>creator</c>.</returns>
        public static T[][] Create<T>(int rows, int cols, Func<int, int, T> creator)
        {
            var ret = new T[rows][];

            Parallel.For(0, ret.Length, row =>
            {
                ret[row] = new T[cols];

                Parallel.For(0, ret[row].Length, col =>
                {
                    ret[row][col] = creator(row, col);
                });
            });

            return ret;
        }

    }
}
