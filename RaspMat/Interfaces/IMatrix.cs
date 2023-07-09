using System;

namespace RaspMat.Interfaces
{
    /// <summary>
    /// A two-dimensional matrix.
    /// </summary>
    /// <typeparam name="TNumber"><see cref="Type"/> of the value in each cell.</typeparam>
    internal interface IMatrix<TNumber>
    {

        /// <summary>
        /// Returns a cell from the matrix with the specified indexes.
        /// </summary>
        /// <param name="row">Index of the row of the matrix.</param>
        /// <param name="column">Index of the column of the matrix.</param>
        /// <returns>Cell with indexes specified.</returns>
        TNumber this[int row, int column] { get; }

        /// <summary>
        /// Returns the number of rows.
        /// </summary>
        int Rows { get; }

        /// <summary>
        /// Returns the number of columns.
        /// </summary>
        int Columns { get; }

    }
}
