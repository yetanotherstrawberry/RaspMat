using System;
using System.Runtime.CompilerServices;

namespace RaspMat.Extensions
{
    /// <summary>
    /// Generic extenstions for <see langword="class"/>es.
    /// </summary>
    internal static class ClassExtensions
    {

        /// <summary>
        /// Will <see langword="return"/> <paramref name="instance"/> or <see langword="throw"/> an <see cref="Exception"/> if <paramref name="instance"/> <see langword="is"/> <see langword="null"/>.
        /// </summary>
        /// <typeparam name="TObject">The instance to <see langword="return"/>.</typeparam>
        /// <param name="instance">The instance that can be <see langword="null"/>.</param>
        /// <param name="message">Message to pass to the <see cref="ArgumentNullException"/>.</param>
        /// <returns>The <paramref name="instance"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> <see langword="is"/> <see langword="null"/>.</exception>
        public static TObject ThrowIfNull<TObject>(this TObject instance, [CallerMemberName] string message = "") where TObject : class
        {
            return instance ?? throw new ArgumentNullException(message);
        }

    }
}
