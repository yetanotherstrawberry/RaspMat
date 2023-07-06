using System;
using System.Data;

namespace RaspMat.Helpers
{
    internal static class DataTableHelpers
    {

        public static DataTable CreateDataTable<T>(int rows, int columns, Func<int, int, T> cellValue)
        {
            var ret = new DataTable();

            for (int col = 0; col < columns; col++)
            {
                ret.Columns.Add(new DataColumn
                {
                    DataType = typeof(string),
                    ColumnName = col.ToString(),
                });
            }

            for (int row = 0; row < rows; row++)
            {
                var dataRow = ret.NewRow();
                for (int col = 0; col < columns; col++)
                    dataRow[col] = cellValue(row, col);

                ret.Rows.Add(dataRow);
            }

            return ret;
        }

    }
}
