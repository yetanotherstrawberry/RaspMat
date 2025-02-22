using CommunityToolkit.Mvvm.Messaging;
using RaspMat.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace RaspMat.Services
{
    /// <summary>
    /// Implements <see cref="IEventService"/> using <see cref="WeakReferenceMessenger"/>.
    /// Usage of <see cref="IDisposable.Dispose"/> returned from <see cref="Subscribe{TEventType}(IObserver{TEventType})"/> is not required.
    /// </summary>
    internal class WeakReferenceMessengerEventService : IEventService
    {

        /// <summary>
        /// Returns <see cref="IObservable{T}"/> based on <see cref="Type"/> of the event.
        /// </summary>
        private readonly IDictionary<Type, object> _observables = new Dictionary<Type, object>();

        /// <summary>
        /// An instance of <see cref="IMessenger"/> to manage subscriptions and messages.
        /// </summary>
        private readonly IMessenger _messenger = WeakReferenceMessenger.Default;

        public IDisposable Subscribe<TEventType>(IObserver<TEventType> observer) where TEventType : class
        {
            if (!_observables.TryGetValue(typeof(TEventType), out var observable))
            {
                _observables.Add(typeof(TEventType), observable = _messenger.CreateObservable<TEventType>());
            }
            return ((IObservable<TEventType>)observable).Subscribe(observer);
        }

        public void Send<TEventType>(TEventType message) where TEventType : class
        {
            _messenger.Send(message);
        }

    }
}
