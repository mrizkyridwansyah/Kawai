using Kawai.Api.Models;
using System;
using System.Data;
using System.Data.Common;

namespace Kawai.Data;

public static class DataTableHelper
{
    public static DataTable ToDataTable<T>(List<T> items, IEnumerable<string>? excludedProperties = null)
    {
        var table = new DataTable(typeof(T).Name);
        var properties = typeof(T).GetProperties()
            .Where(p => excludedProperties == null || !excludedProperties.Contains(p.Name))
            .ToArray();

        foreach (var prop in properties)
        {
            table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        }

        foreach (var item in items)
        {
            var values = new object[properties.Length];
            for (int i = 0; i < properties.Length; i++)
            {
                values[i] = properties[i].GetValue(item) ?? DBNull.Value;
            }
            table.Rows.Add(values);
        }
        return table;
    }

    /// <summary>
    /// Convert List of string to DataTable with single column. 
    /// Contoh : List string ke User-Defined Table dengan kolom cuma "BarcodeNo"
    /// </summary>
    public static DataTable ToDataTableSingle(List<string> items, string columnName)
    {
        var table = new DataTable();
        table.Columns.Add(columnName, typeof(string));

        foreach (var item in items)
        {
            table.Rows.Add(item);
        }

        return table;
    }
}
