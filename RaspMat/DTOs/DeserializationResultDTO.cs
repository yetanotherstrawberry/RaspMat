using System;
using System.Collections.Generic;

namespace RaspMat.DTOs
{
    /// <summary>
    /// Result of a deserialization.
    /// </summary>
    /// <typeparam name="TDeserialized"><see cref="Type"/> of <see cref="DeserializationResultDTO{TDeserialized}.Result"/>.</typeparam>
    internal class DeserializationResultDTO<TDeserialized>
    {

        /// <summary>
        /// Result of the deserialization or default value of <see cref="TDeserialized"/> if <see cref="Successful"/> is <see langword="false"/>.
        /// </summary>
        public TDeserialized Result { get; }

        /// <summary>
        /// Indicates whether deserialization was successful. Returns <see langword="true"/> if it was.
        /// </summary>
        public bool Successful { get; }

        /// <summary>
        /// Creates a new <see cref="DeserializationResultDTO{TDeserialized}"/>. Use empty constructor for failure or set both parameters otherwise.
        /// </summary>
        /// <param name="successful">Indicates whether deserialization was successful.</param>
        /// <param name="result">Result of the deserialization. Must be equal to defaul value if <paramref name="successful"/> is <see langword="false"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="successful"/> is <see langword="false"/> and <paramref name="result"/> is not a default value.</exception>
        public DeserializationResultDTO(bool successful = false, TDeserialized result = default)
        {
            if (!successful && !EqualityComparer<TDeserialized>.Default.Equals(result, default))
            {
                throw new ArgumentException();
            }

            Successful = successful;
            Result = result;
        }

    }
}
