using RaspMat.Interfaces;
using System;
using System.Collections.Generic;

namespace RaspMat.Models
{
    /// <summary>
    /// Result of a deserialization.
    /// </summary>
    /// <typeparam name="TDeserialized"><see cref="Type"/> of <see cref="DeserializationResult{TDeserialized}.Result"/>.</typeparam>
    internal class DeserializationResult<TDeserialized> : IDeserializationResult<TDeserialized>
    {

        public TDeserialized Result { get; }
        public bool Successful { get; }

        /// <summary>
        /// Creates a new <see cref="DeserializationResult{TDeserialized}"/>. Use empty constructor for failure or set both parameters otherwise.
        /// </summary>
        /// <param name="successful">Indicates whether deserialization was successful.</param>
        /// <param name="result">Result of the deserialization. Must be equal to defaul value if <paramref name="successful"/> is <see langword="false"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="successful"/> is <see langword="false"/> and <paramref name="result"/> is not a default value.</exception>
        public DeserializationResult(bool successful = false, TDeserialized result = default)
        {
            Successful = successful;
            Result = result;

            if (!successful && !EqualityComparer<TDeserialized>.Default.Equals(result, default))
            {
                throw new ArgumentException();
            }
        }

    }
}
