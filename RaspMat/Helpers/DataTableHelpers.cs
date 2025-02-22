using System;
using System.Data;
using System.Linq;

namespace RaspMat.Helpers
{
    /// <summary>
    /// Static class for helpers for <see cref="DataTable"/>s.
    /// </summary>
    internal static class DataTableHelpers
    {

        /// <summary>
        /// Creates a <see cref="DataTable"/> with <see cref="string"/> <see cref="DataColumn.DataType"/> and values from <paramref name="cellValue"/>.
        /// </summary>
        /// <typeparam name="TValue"><see cref="Type"/> of values returned by <paramref name="cellValue"/>.</typeparam>
        /// <param name="rows">Number of rows in the <see cref="DataTable"/>. Will iterate the <paramref name="cellValue"/> from 0 to <paramref name="rows"/> exclusive.</param>
        /// <param name="columns">Number of columns in the <see cref="DataTable"/>. Will iterate the <paramref name="cellValue"/> from 0 to <paramref name="columns"/> exclusive.</param>
        /// <param name="cellValue">A <see cref="Func{TRow, TColumn, TValue}"/> that returns a value for every row and column. Values must implement <see cref="object.ToString"/>.</param>
        /// <returns>A <paramref name="rows"/> by <paramref name="columns"/> <see cref="DataTable"/> with values from <paramref name="cellValue"/>.</returns>
        public static DataTable CreateStrDataTable<TValue>(this Func<int, int, TValue> cellValue, int rows, int columns)
        {
            var ret = new DataTable();

            ret.Columns.AddRange(Enumerable.Range(0, columns).Select(columnIndex => new DataColumn
            {
                DataType = typeof(string),
                ColumnName = columnIndex.ToString(),
            }).ToArray());

            for (var row = 0; row < rows; row++)
            {
                var dataRow = ret.NewRow();
                dataRow.ItemArray = Enumerable.Range(0, columns).Select(columnIndex => cellValue(row, columnIndex).ToString()).Cast<object>().ToArray();
                ret.Rows.Add(dataRow);
            }

            return ret;
        }

    }
}
