using RaspMat.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace RaspMat.Models
{
    /// <summary>
    /// The <see cref="Matrix"/> of probabilities for <see cref="Indexes"/>.
    /// </summary>
    [Serializable]
    internal class ProbabilityChain : Matrix
    {

        /// <summary>
        /// Maps node labels to indexes in the <see cref="Matrix.FractionMatrix"/>.
        /// </summary>
        private IDictionary<string, int> Indexes { get; }

        public ProbabilityChain(Matrix matrix, IEnumerable<string> labels) : base(matrix.Rows, matrix.Columns, (row, column) => matrix[row, column])
        {
            Indexes = labels is null ? Enumerable.Range(0, matrix.Columns).ToDictionary(index => index.ToString()) : labels.Select((label, index) => new KeyValuePair<string, int>(label, index)).ToDictionary();
        }

        protected internal ProbabilityChain(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Indexes = (IDictionary<string, int>)info.GetValue(nameof(Indexes), typeof(IDictionary<string, int>));
        }

        /// <summary>
        /// Gets the fraction value associated with the specified source and target index names.
        /// </summary>
        /// <remarks>Both source and target must exist in the Indexes collection; otherwise, an exception
        /// may be thrown.</remarks>
        /// <param name="source">The name of the source index used to locate the corresponding fraction.</param>
        /// <param name="target">The name of the target index used to locate the corresponding fraction.</param>
        /// <returns>A Fraction object representing the value at the intersection of the specified source and target indices.</returns>
        public Fraction this[string source, string target] => base[Indexes[source], Indexes[target]];

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, int>> GetIndexes() => Indexes.OrderBy(kvp => kvp.Value);

        /// <inheritdoc/>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Indexes), Indexes, Indexes.GetType());
            base.GetObjectData(info, context);
        }

        /// <inheritdoc/>
        public override object Clone()
        {
            return new ProbabilityChain(this, GetIndexes().Select(kvp => kvp.Key));
        }

    }
}
