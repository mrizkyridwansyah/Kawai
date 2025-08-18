using System.Data;

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
}
