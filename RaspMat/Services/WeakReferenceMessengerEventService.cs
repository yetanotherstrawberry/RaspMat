using CommunityToolkit.Mvvm.Messaging;
using RaspMat.Services.Interfaces;

namespace RaspMat.Services
{
    /// <summary>
    /// Implements <see cref="IEventService"/> using <see cref="WeakReferenceMessenger"/>.
    /// </summary>
    internal class WeakReferenceMessengerEventService : IEventService
    {

        /// <summary>
        /// An instance of <see cref="IMessenger"/> to manage subscriptions and messages.
        /// </summary>
        private readonly IMessenger _messenger = WeakReferenceMessenger.Default;

        public void Subscribe<TObserver, TEventType>(TObserver observer) where TObserver : class, IEventReceiver<TEventType> where TEventType : class
        {
            _messenger.Register(observer, new MessageHandler<TObserver, TEventType>((receiver, message) => observer.Receive(message)));
        }

        public void Unsubscribe<TObserver, TEventType>(TObserver observer) where TObserver : class, IEventReceiver<TEventType> where TEventType : class
        {
            _messenger.Unregister<TEventType>(observer);
        }

        public void Send<TEventType>(TEventType message) where TEventType : class
        {
            _messenger.Send(message);
        }

    }
}
