using RaspMat.Models;
using System.Collections.Generic;

namespace RaspMat.Helpers
{
    internal static class Events
    {

        public abstract class Event<TData>
        {
            public TData Data { get; }

            protected Event(TData data)
            {
                Data = data;
            }
        }

        public sealed class LoadMatrixEvent : Event<Matrix>
        {
            public LoadMatrixEvent(Matrix data) : base(data) { }
        }

        public sealed class LoadStepsEvent : Event<IList<AlgorithmStep<Matrix>>>
        {
            public LoadStepsEvent(IList<AlgorithmStep<Matrix>> data) : base(data) { }
        }

        public sealed class OperationPerformedEvent : Event<string>
        {
            public OperationPerformedEvent(string data) : base(data) { }
        }

    }
}
