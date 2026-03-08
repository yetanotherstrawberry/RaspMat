using System.Collections.Generic;

namespace RaspMat.Models
{
    /// <summary>
    /// Container <see langword="class"/> for <see cref="Events"/>.
    /// </summary>
    internal static class Events
    {

        /// <summary>
        /// Represents an <see cref="Event{TData}"/> of a <typeparamref name="TData"/> parameter.
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        public abstract class Event<TData>
        {
            /// <summary>
            /// Parameter of the <see cref="Event{TData}"/>.
            /// </summary>
            public TData Data { get; }

            public Event(TData data)
            {
                Data = data;
            }
        }

        /// <summary>
        /// Describes the load of a <see cref="Matrix"/>.
        /// </summary>
        public sealed class LoadMatrixEvent : Event<Matrix>
        {
            public LoadMatrixEvent(Matrix data) : base(data) { }
        }

        /// <summary>
        /// Describes the load of <see cref="AlgorithmStep{TResult}"/>s.
        /// </summary>
        public sealed class LoadStepsEvent : Event<IList<AlgorithmStep<Matrix>>>
        {
            public LoadStepsEvent(IList<AlgorithmStep<Matrix>> data) : base(data) { }
        }

        /// <summary>
        /// Describes an operation.
        /// </summary>
        public sealed class OperationPerformedEvent : Event<string>
        {
            public OperationPerformedEvent(string data) : base(data) { }
        }

    }
}
