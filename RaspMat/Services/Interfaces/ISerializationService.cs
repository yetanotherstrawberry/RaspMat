using System.Threading.Tasks;

namespace RaspMat.Services.Interfaces
{
    /// <summary>
    /// Interface for service (de)serializing <see cref="object"/>s.
    /// </summary>
    internal interface ISerializationService
    {

        /// <summary>
        /// Serializes <paramref name="instance"/>.
        /// </summary>
        /// <typeparam name="TSerialized">Serialized entity.</typeparam>
        /// <param name="instance">The <see cref="object"/> to serialize.</param>
        /// <returns>Nothing.</returns>
        Task SerializeAsync<TSerialized>(TSerialized instance) where TSerialized : class; // Constraint forces value to be nullable.

        /// <summary>
        /// Requests deserialization and returns an instance of <typeparamref name="TDeserialized"/> or <see langword="null"/>.
        /// </summary>
        /// <typeparam name="TDeserialized">Requested <see cref="object"/>.</typeparam>
        /// <returns>An instance of <typeparamref name="TDeserialized"/> or <see langword="null"/>.</returns>
        Task<TDeserialized> DeserializeAsync<TDeserialized>() where TDeserialized : class; // Constraint forces value to be nullable.

    }
}
