namespace RaspMat.Models
{
    /// <summary>
    /// Describes a <see cref="Matrix"/> for creation. Does not validate properties.
    /// </summary>
    internal readonly struct NewMatrixDescription
    {

        /// <summary>
        /// The number of rows to create.
        /// </summary>
        public int Rows { get; }

        /// <summary>
        /// The number of columns to create.
        /// </summary>
        public int Columns { get; }

        /// <summary>
        /// Indicates whether the <see cref="Matrix"/> should be filled with zeros.
        /// </summary>
        public bool FillWithZeros { get; }

    }
}
