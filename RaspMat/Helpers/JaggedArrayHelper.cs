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
        /// Converts a 2D jagged array.
        /// </summary>
        /// <typeparam name="TSource">Input array type.</typeparam>
        /// <typeparam name="TDestination">Type of the returned array.</typeparam>
        /// <param name="input">Array to be converted. Will not be modified.</param>
        /// <param name="converter">Converter changing the type for a single element of the input array.</param>
        /// <exception cref="InvalidOperationException">The execution of <see cref="Parallel.For(int, int, Action{int})"/> did not complete as <see cref="ParallelLoopResult.IsCompleted"/> is false.</exception>
        /// <returns>New array resulted from converting the input.</returns>
        public static TDestination[][] Convert<TSource, TDestination>(this TSource[][] input, Converter<TSource, TDestination> converter)
        {
            var converted = new TDestination[input.Length][];
            if (!Parallel.For(0, converted.Length, row =>
            {
                converted[row] = Array.ConvertAll(input[row], converter);
            }).IsCompleted)
            {
                throw new InvalidOperationException(nameof(ParallelLoopResult.IsCompleted));
            }
            return converted;
        }

        /// <summary>
        /// Creates a 2D array.
        /// </summary>
        /// <typeparam name="TElement">Type of the array.</typeparam>
        /// <param name="rows">Length of the 1st dimension.</param>
        /// <param name="cols">Length od the 2nd dimension.</param>
        /// <param name="creator">Function that accepts 2 indexes and returns value to be assigned.</param>
        /// <returns>2D array of <c>T</c> with values from <c>creator</c>.</returns>
        public static TElement[][] Create<TElement>(int rows, int cols, Func<int, int, TElement> creator)
        {
            var ret = new TElement[rows][];

            Parallel.For(0, ret.Length, row =>
            {
                ret[row] = new TElement[cols];

                Parallel.For(0, ret[row].Length, col =>
                {
                    ret[row][col] = creator(row, col);
                });
            });

            return ret;
        }

    }
}
