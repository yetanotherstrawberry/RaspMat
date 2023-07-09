namespace RaspMat.Interfaces
{
    /// <summary>
    /// Interface for results of deserialization.
    /// </summary>
    /// <typeparam name="TDeserialized">Type of the returned value of <see cref="IDeserializationResult{TDeserialized}.Result"/>.</typeparam>
    internal interface IDeserializationResult<TDeserialized>
    {

        /// <summary>
        /// Result of the deserialization or default value of <see cref="TDeserialized"/> if <see cref="Successful"/> is <see langword="false"/>.
        /// </summary>
        TDeserialized Result { get; }

        /// <summary>
        /// Indicates whether deserialization was successful. Returns <see langword="true"/> if it was.
        /// </summary>
        bool Successful { get; }

    }
}
