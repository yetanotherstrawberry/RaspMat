namespace RaspMat.Services.Interfaces
{
    /// <summary>
    /// Use to communicate between views.
    /// </summary>
    /// <typeparam name="TEventType">The message.</typeparam>
    internal interface IEventReceiver<TEventType>
    {

        /// <summary>
        /// Receives the <typeparamref name="TEventType"/> <paramref name="message"/>.
        /// </summary>
        /// <param name="message">An instance of a message.</param>
        void Receive(TEventType message);

    }
}
