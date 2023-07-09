using System;

namespace RaspMat.Interfaces
{
    /// <summary>
    /// Interface for service (de)serializing instances.
    /// </summary>
    internal interface ISerializationService
    {

        /// <summary>
        /// Serializes <paramref name="instance"/>.
        /// </summary>
        /// <typeparam name="TDeserialized"><see cref="Type"/> of <paramref name="instance"/>.</typeparam>
        /// <param name="instance">An instance to be serialized.</param>
        void Serialize<TDeserialized>(TDeserialized instance);

        /// <summary>
        /// Requests deserialization and returns an instance of <typeparamref name="TDeserialized"/>.
        /// </summary>
        /// <typeparam name="TDeserialized"><see cref="Type"/> of <see cref="IDeserializationResult{TDeserialized}.Result"/>.</typeparam>
        /// <returns>
        /// <see cref="IDeserializationResult{TDeserialized}"/> of <typeparamref name="TDeserialized"/>.
        /// <see cref="IDeserializationResult{TDeserialized}.Result"/> indicates whether deserialization was successful.
        /// If it is <see langword="false"/>, <see cref="IDeserializationResult{TDeserialized}.Result"/> will be equal to default.
        /// </returns>
        IDeserializationResult<TDeserialized> Deserialize<TDeserialized>();

    }
}
