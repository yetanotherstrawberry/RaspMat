namespace RaspMat.Services.Interfaces
{
    /// <summary>
    /// A service for handling messaging.
    /// </summary>
    internal interface IEventService
    {

        /// <summary>
        /// Registers a listener.
        /// </summary>
        /// <typeparam name="TObserver">The observer.</typeparam>
        /// <typeparam name="TEventType">Message to send to the <paramref name="observer"/>.</typeparam>
        /// <param name="observer">An <see cref="object"/> to receive messages.</param>
        void Subscribe<TObserver, TEventType>(TObserver observer) where TObserver : class, IEventReceiver<TEventType> where TEventType : class;

        /// <summary>
        /// Unregisters a listener.
        /// </summary>
        /// <typeparam name="TObserver">The observer.</typeparam>
        /// <typeparam name="TEventType">Message the <paramref name="observer"/> listened for.</typeparam>
        /// <param name="observer">An <see cref="object"/> to stop receiving messages.</param>
        void Unsubscribe<TObserver, TEventType>(TObserver observer) where TObserver : class, IEventReceiver<TEventType> where TEventType : class;

        /// <summary>
        /// Sends a message to the subscribers. Use <see cref="Subscribe{TEventType}(IObserver{TEventType})"/> to subscribe.
        /// </summary>
        /// <typeparam name="TEventType"></typeparam>
        /// <param name="message">An instance of <typeparamref name="TEventType"/> to send.</param>
        void Send<TEventType>(TEventType message) where TEventType : class;

    }
}
