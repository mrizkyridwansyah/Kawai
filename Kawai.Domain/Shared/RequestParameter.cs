using System.ComponentModel.DataAnnotations;
using System.Dynamic;

namespace Kawai.Domain.Shared;
public class RequestParameter
{
    public int? Page { get; set; } = 1;
    public int? Length { get; set; } = 10;

    //public List<KeyValuePair<string, string>> Sorts { get; set; } = new();
    public Dictionary<string, string> Sorts { get; set; } = new();
    public List<Dictionary<string, string>> Filters { get; set; } = new();

    public object ToQueryObject()
    {
        dynamic obj = new ExpandoObject();
        var dict = (IDictionary<string, object>)obj;

        // Add paging
        dict["Page"] = Page;
        dict["Length"] = Length;

        // Add sort
        dict["Sort"] = (Sorts == null || Sorts.Count == 0) ? "" : string.Join(", ", Sorts.Select(entry => $"{entry.Key} {entry.Value}"));

        // Add filters (flatten all filter entries)
        foreach (var filter in Filters)
        {
            foreach (var pair in filter)
            {
                if (!dict.ContainsKey(pair.Key))
                    dict[pair.Key] = pair.Value;
            }
        }

        return obj;
    }
}
