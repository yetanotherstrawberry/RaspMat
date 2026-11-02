using System;
using System.Data;
using System.Linq;

namespace RaspMat.Extensions
{
    internal static class DataTableExtensions
    {

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

        public static DataTable SetRows(this DataTable dataTable, ushort count, Func<int, int, object> rowFactory = null)
        {
            if (rowFactory is null) rowFactory = (rowIndex, colIndex) => rowIndex == colIndex ? "1" : "0";
            dataTable.Rows.Clear();
            foreach (var row in Enumerable.Range(0, count).Select(index =>
            {
                var row = dataTable.NewRow();
                row.ItemArray = Enumerable.Range(0, dataTable.Columns.Count).Select(colIndex => rowFactory(index, colIndex)).ToArray();
                return row;
            }))
            {
                dataTable.Rows.Add(row);
            }
            return dataTable;
        }

    }
}
