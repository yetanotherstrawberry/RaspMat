using System;
using System.Collections.Generic;

namespace RaspMat.Extensions
{
    /// <summary>
    /// Extensions for the <see cref="Exception"/> <see langword="class"/>.
    /// </summary>
    internal static class ExceptionExtensions
    {

        /// <summary>
        /// Unwraps the <see cref="Exception"/> to get the most <see cref="Exception.InnerException"/>.
        /// </summary>
        /// <param name="exception">The exception to unwrap.</param>
        /// <returns>The deepest <see cref="Exception"/>.</returns>
        public static Exception Unwrap(this Exception exception)
        {
            var unwrapped = new HashSet<Exception>();
            while (exception.InnerException != null && unwrapped.Add(exception)) exception = exception.InnerException;
            return exception;
        }

    }
}
