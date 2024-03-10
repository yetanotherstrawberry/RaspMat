using Prism.Events;
using RaspMat.Models;
using System;
using System.Collections.Generic;

namespace RaspMat.Helpers
{
    internal static class Events
    {

        public abstract class Event<TInput> : PubSubEvent<TInput>
        {
            public new SubscriptionToken Subscribe(Action<TInput> action)
            {
                return Subscribe(action, ThreadOption.UIThread, keepSubscriberReferenceAlive: false);
            }
        }

        public class LoadMatrixEvent : Event<Matrix> { }

        public class LoadStepsEvent : Event<ICollection<AlgorithmStep<Matrix>>> { }

    }
}
