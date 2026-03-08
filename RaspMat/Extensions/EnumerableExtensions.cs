using RaspMat.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RaspMat.Extensions
{
    /// <summary>
    /// A <see langword="class"/> for the <see cref="IEnumerable{T}"/> and related <see langword="class"/>es.
    /// </summary>
    internal static class EnumerableExtensions
    {

        /// <summary>
        /// Creates a <see langword="new"/> <see cref="IDictionary{TKey, TValue}"/> from the <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="TKey">The <see cref="IDictionary{TKey, TValue}.Keys"/>.</typeparam>
        /// <typeparam name="TValue">The <see cref="IDictionary{TKey, TValue}.Values"/>.</typeparam>
        /// <param name="source">The elements.</param>
        /// <returns>A <see langword="new"/> <see cref="IDictionary{TKey, TValue}"/>.</returns>
        public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            return source.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        /// <summary>
        /// Projects each element and calculates the sum of a sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
        /// <param name="source">The sequence.</param>
        /// <param name="selector">The projection.</param>
        /// <returns>A <see cref="Fraction"/>.</returns>
        public static Fraction Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, Fraction> selector)
        {
            return source.Select(selector).Sum();
        }

        /// <summary>
        /// Calculates the sum of a sequence.
        /// </summary>
        /// <param name="source">The sequence.</param>
        /// <returns>A <see cref="Fraction"/>.</returns>
        public static Fraction Sum(this IEnumerable<Fraction> source)
        {
            return source.Aggregate((left, right) => left + right);
        }

    }
}
