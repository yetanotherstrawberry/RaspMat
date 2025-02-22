using System;

namespace RaspMat.Services.Interfaces
{
    /// <summary>
    /// A service for handling messaging.
    /// </summary>
    internal interface IEventService
    {

        /// <summary>
        /// Regirsters the <paramref name="observer"/> to listen for <typeparamref name="TEventType"/> messages.
        /// </summary>
        /// <typeparam name="TEventType">Message</typeparam>
        /// <param name="observer">An instance of <see cref="IObserver{T}"/> that will receive <typeparamref name="TEventType"/> messages.</param>
        /// <returns>An <see cref="IDisposable"/>. Use <see cref="IDisposable.Dispose"/> to unsubscribe.</returns>
        IDisposable Subscribe<TEventType>(IObserver<TEventType> observer) where TEventType : class;

        /// <summary>
        /// Sends a message to the subscribers. Use <see cref="Subscribe{TEventType}(IObserver{TEventType})"/> to subscribe.
        /// </summary>
        /// <typeparam name="TEventType"></typeparam>
        /// <param name="message">An instance of <typeparamref name="TEventType"/> to send.</param>
        void Send<TEventType>(TEventType message) where TEventType : class;

    }
}
