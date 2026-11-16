using System;
using System.Data;
using System.Linq;

namespace RaspMat.Extensions
{
    /// <summary>
    /// Extensions for the <see cref="DataTable"/> <see langword="class"/>.
    /// </summary>
    internal static class DataTableExtensions
    {

        /// <summary>
        /// Removes all columns and adds the specified <paramref name="count"/> of newly created ones.
        /// </summary>
        /// <param name="dataTable">The <see cref="DataTable"/> to modify.</param>
        /// <param name="count">The numbers of columns.</param>
        /// <returns>The <paramref name="dataTable"/>.</returns>
        public static DataTable SetColumns(this DataTable dataTable, ushort count)
        {
            dataTable.Columns.Clear();
            dataTable.Columns.AddRange(Enumerable.Range(1, count).Select(index => new DataColumn
            {
                ColumnName = index.ToString(),
                DataType = typeof(string),
            }).ToArray());
            return dataTable;
        }

        /// <summary>
        /// Removes all rows and adds the specified <paramref name="count"/> of newly created ones.
        /// </summary>
        /// <param name="dataTable">The <see cref="DataTable"/> to modify.</param>
        /// <param name="count">The number of rows to add.</param>
        /// <param name="factory">A function that creates a new row.</param>
        /// <returns>The <paramref name="dataTable"/>.</returns>
        public static DataTable SetRows(this DataTable dataTable, ushort count, Func<int, int, object> factory = null)
        {
            if (factory is null) factory = (rowIndex, colIndex) => rowIndex == colIndex ? 1 : 0;
            dataTable.Rows.Clear();
            foreach (var row in Enumerable.Range(0, count).Select(index =>
            {
                var row = dataTable.NewRow();
                row.ItemArray = Enumerable.Range(0, dataTable.Columns.Count).Select(colIndex => factory(index, colIndex).ToString()).ToArray();
                return row;
            }))
            {
                dataTable.Rows.Add(row);
            }
            return dataTable;
        }

    }
}
