using System.Text.RegularExpressions;

namespace RaspMat.Extensions
{
    /// <summary>
    /// Extensions for <see cref="string"/>s.
    /// </summary>
    internal static class StringExtensions
    {

        /// <summary>
        /// Used for matching white characters.
        /// </summary>
        private static readonly Regex _whiteSpaceRegex = new Regex(@"\s+", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>
        /// Removes all white characters.
        /// </summary>
        /// <param name="input">Input <see cref="string"/> to process.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string RemoveWhitespaces(this string input)
        {
            return input is null ? null : _whiteSpaceRegex.Replace(input, string.Empty);
        }

    }
}
