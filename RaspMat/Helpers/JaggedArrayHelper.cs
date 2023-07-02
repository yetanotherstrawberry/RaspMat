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
        /// <param name="colLength">Designated length of the 2nd dimension. Will copy everything past starting index if null.</param>
        /// <returns>New array with elements copied from input.</returns>
        public static T[][] Copy<T>(this T[][] input, long startCol = 0, long? colLength = null)
        {
            var ret = new T[input.LongLength][];

            Parallel.For(0, ret.LongLength, row =>
            {
                ret[row] = new T[colLength ?? input[row].LongLength - startCol];
                Array.Copy(input[row], startCol, ret[row], 0, ret[row].LongLength);
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
            var ret = new Y[input.LongLength][];

            Parallel.For(0, ret.LongLength, row =>
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
        public static T[][] Create<T>(long rows, long cols, Func<long, long, T> creator)
        {
            var ret = new T[rows][];

            Parallel.For(0, ret.LongLength, row =>
            {
                ret[row] = new T[cols];

                Parallel.For(0, ret[row].LongLength, col =>
                {
                    ret[row][col] = creator(row, col);
                });
            });

            return ret;
        }

    }
}
