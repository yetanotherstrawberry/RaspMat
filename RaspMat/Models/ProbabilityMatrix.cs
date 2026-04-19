using RaspMat.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace RaspMat.Models
{
    /// <summary>
    /// The <see cref="Matrix"/> of probabilities for <see cref="TransientIndexes"/>.
    /// </summary>
    [Serializable]
    internal class ProbabilityMatrix : Matrix
    {

        /// <summary>
        /// Maps transient nodes' labels to row indexe in the <see cref="Matrix.FractionMatrix"/>.
        /// </summary>
        private IDictionary<string, int> TransientIndexes { get; }

        /// <summary>
        /// Maps exit nodes' labels to indexes in the <see cref="Matrix.FractionMatrix"/>.
        /// </summary>
        private IDictionary<string, int> ExitIndexes { get; }

        /// <summary>
        /// Gets the source nodes.
        /// </summary>
        public IEnumerable<string> Sources => TransientIndexes.Keys;

        /// <summary>
        /// Gets the exit nodes.
        /// </summary>
        public IEnumerable<string> Targets => ExitIndexes.Keys;

        public ProbabilityMatrix(Matrix matrix, IEnumerable<string> transientLabels, IEnumerable<string> exitLabels) : base(matrix.Rows, matrix.Columns, (row, column) => matrix[row, column])
        {
            if (!transientLabels.ThrowIfNull().Any()) throw new ArgumentNullException(nameof(transientLabels));
            if (!exitLabels.ThrowIfNull().Any()) throw new ArgumentNullException(nameof(exitLabels));
            TransientIndexes = transientLabels.Select((label, index) => new KeyValuePair<string, int>(label, index)).ToDictionary();
            ExitIndexes = exitLabels.Select((label, index) => new KeyValuePair<string, int>(label, index)).ToDictionary();
        }

        protected internal ProbabilityMatrix(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            TransientIndexes = (IDictionary<string, int>)info.GetValue(nameof(TransientIndexes), typeof(IDictionary<string, int>));
            ExitIndexes = (IDictionary<string, int>)info.GetValue(nameof(ExitIndexes), typeof(IDictionary<string, int>));
        }

        /// <summary>
        /// Gets the probability of transitioning from <paramref name="source"/> to <paramref name="target"/>.
        /// </summary>
        /// <param name="source">Label of the source node.</param>
        /// <param name="target">Label of the exit node.</param>
        /// <returns>A <see cref="Fraction"/>.</returns>
        public Fraction this[string source, string target] => base[TransientIndexes[source], TransientIndexes.Count + ExitIndexes[target]];

        /// <inheritdoc/>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(TransientIndexes), TransientIndexes, TransientIndexes.GetType());
            info.AddValue(nameof(ExitIndexes), ExitIndexes, ExitIndexes.GetType());
            base.GetObjectData(info, context);
        }

        /// <inheritdoc/>
        public override object Clone()
        {
            return new ProbabilityMatrix(this, TransientIndexes.OrderBy(kvp => kvp.Value).Select(kvp => kvp.Key), ExitIndexes.OrderBy(kvp => kvp.Value).Select(kvp => kvp.Key));
        }

    }
}
