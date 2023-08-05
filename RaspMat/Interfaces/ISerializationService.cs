using RaspMat.DTOs;
using System;
using System.IO;

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
        /// <param name="instance">An instance to be serialized.</param
        /// <param name="stream"><see cref="Stream"/> to serialize to.</param>
        void Serialize<TDeserialized>(TDeserialized instance);

        /// <summary>
        /// Requests deserialization and returns an instance of <typeparamref name="TDeserialized"/>.
        /// </summary>
        /// <typeparam name="TDeserialized"><see cref="Type"/> of <see cref="IDeserializationResult{TDeserialized}.Result"/>.</typeparam>
        /// <param name="stream"><see cref="Stream"/> to the deserialization source.</param>
        /// <returns>
        /// <see cref="IDeserializationResult{TDeserialized}"/> of <typeparamref name="TDeserialized"/>.
        /// <see cref="IDeserializationResult{TDeserialized}.Result"/> indicates whether deserialization was successful.
        /// If it is <see langword="false"/>, <see cref="IDeserializationResult{TDeserialized}.Result"/> will be equal to default.
        /// </returns>
        DeserializationResultDTO<TDeserialized> Deserialize<TDeserialized>();

    }
}
