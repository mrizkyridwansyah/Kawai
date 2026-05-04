using System.Reflection;
using System.Text.Json;

public static class ObjectComparer
{
    public static Dictionary<string, object?> Compare(object? before, object? after)
    {
        var differences = new Dictionary<string, object?>();

        if (before == null && after == null)
            return differences;

        if (before == null)
        {
            differences["Added"] = after;
            return differences;
        }

        if (after == null)
        {
            differences["Removed"] = before;
            return differences;
        }

        var type = before.GetType();

        // Kalau dictionary (misal JSON di-deserialize)
        if (before is IDictionary<string, object> dictBefore &&
            after is IDictionary<string, object> dictAfter)
        {
            foreach (var key in dictBefore.Keys.Union(dictAfter.Keys))
            {
                dictBefore.TryGetValue(key, out var bVal);
                dictAfter.TryGetValue(key, out var aVal);

                if (!AreEqual(bVal, aVal))
                    differences[key] = new { Before = bVal, After = aVal };
            }
            return differences;
        }

        // Kalau enumerable (list)
        if (before is IEnumerable<object> listBefore && after is IEnumerable<object> listAfter)
        {
            var diffList = listBefore.Zip(listAfter, (b, a) => Compare(b, a))
                .Where(d => d.Count > 0)
                .ToList();

            if (diffList.Count > 0)
                differences["ListChanges"] = diffList;

            return differences;
        }

        // Kalau object biasa
        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var bVal = prop.GetValue(before);
            var aVal = prop.GetValue(after);

            if (!AreEqual(bVal, aVal))
                differences[prop.Name] = new { Before = bVal, After = aVal };
        }

        return differences;
    }

    private static bool AreEqual(object? a, object? b)
    {
        if (a == null && b == null)
            return true;

        if (a == null || b == null)
            return false;

        if (a.Equals(b))
            return true;

        // Serialize to string comparison (handle JSON/dynamic differences)
        return JsonSerializer.Serialize(a) == JsonSerializer.Serialize(b);
    }
}

public static class DictionaryComparer
{
    public static Dictionary<string, object?> CompareDict(
        Dictionary<string, object> before,
        Dictionary<string, object> after)
    {
        var diff = new Dictionary<string, object?>();

        foreach (var key in before.Keys.Union(after.Keys))
        {
            before.TryGetValue(key, out var bVal);
            after.TryGetValue(key, out var aVal);

            if (!JsonSerializer.Serialize(bVal).Equals(JsonSerializer.Serialize(aVal)))
                diff[key] = new { Before = bVal, After = aVal };
        }

        return diff;
    }
}

public static class AuditComparer
{
    public static Dictionary<string, Dictionary<string, object?>> CompareForLog(
        Dictionary<string, object?>? before,
        Dictionary<string, object?>? after)
    {
        var result = new Dictionary<string, Dictionary<string, object?>>()
        {
            ["Before"] = new Dictionary<string, object?>(),
            ["After"] = new Dictionary<string, object?>()
        };

        if (before == null && after == null)
            return result;

        var allKeys = new HashSet<string>();

        if (before != null)
            foreach (var key in before.Keys)
                allKeys.Add(key);

        if (after != null)
            foreach (var key in after.Keys)
                allKeys.Add(key);

        foreach (var key in allKeys)
        {
            object? bVal = null;
            object? aVal = null;

            if (before != null)
                before.TryGetValue(key, out bVal);

            if (after != null)
                after.TryGetValue(key, out aVal);

            if (!AreEqual(bVal, aVal))
            {
                if (bVal != null)
                    result["Before"][key] = bVal;
                if (aVal != null)
                    result["After"][key] = aVal;
            }
        }

        // hapus section kosong
        if (result["Before"].Count == 0)
            result.Remove("Before");
        if (result["After"].Count == 0)
            result.Remove("After");

        return result;
    }

    private static bool AreEqual(object? a, object? b)
    {
        if (a == null && b == null) return true;
        if (a == null || b == null) return false;

        // Kalau value type biasa
        if (a.Equals(b)) return true;

        // Kalau complex type, compare via JSON
        var jsonA = JsonSerializer.Serialize(a);
        var jsonB = JsonSerializer.Serialize(b);
        return jsonA == jsonB;
    }
}


public static class DataLogComparer
{
    public static (object Before, object After) Compare(object? before, object? after)
    {
        return CompareInternal(before, after);
    }

    private static (object, object) CompareInternal(object? before, object? after)
    {
        if (before == null && after == null)
            return (new Dictionary<string, object>(), new Dictionary<string, object>());

        if (before == null || after == null)
            return (before ?? new object(), after ?? new object());

        // Dictionary case
        if (before is IDictionary<string, object> bDict && after is IDictionary<string, object> aDict)
        {
            var beforeDiff = new Dictionary<string, object>();
            var afterDiff = new Dictionary<string, object>();

            var keys = new HashSet<string>(bDict.Keys.Concat(aDict.Keys));
            foreach (var key in keys)
            {
                bDict.TryGetValue(key, out var bVal);
                aDict.TryGetValue(key, out var aVal);

                if (!AreEqual(bVal, aVal))
                {
                    var (bSub, aSub) = CompareInternal(bVal, aVal);
                    beforeDiff[key] = bSub;
                    afterDiff[key] = aSub;
                }
            }

            return (beforeDiff, afterDiff);
        }

        // List case
        if (before is IEnumerable<object> bList && after is IEnumerable<object> aList)
        {
            var bArr = bList.Cast<object>().ToList();
            var aArr = aList.Cast<object>().ToList();

            if (bArr.Count != aArr.Count || !bArr.SequenceEqual(aArr, new ObjectComparer()))
                return (bArr, aArr);
            else
                return (new List<object>(), new List<object>());
        }

        // Primitive / fallback
        if (!AreEqual(before, after))
            return (before, after);

        return (new Dictionary<string, object>(), new Dictionary<string, object>());
    }

    private static bool AreEqual(object? a, object? b)
    {
        if (a == null && b == null) return true;
        if (a == null || b == null) return false;

        // Serialize to JSON for deep equality (works for nested objects)
        var jsonA = JsonSerializer.Serialize(a);
        var jsonB = JsonSerializer.Serialize(b);

        return jsonA == jsonB;
    }

    // Comparer for list elements
    private class ObjectComparer : IEqualityComparer<object>
    {
        public new bool Equals(object? x, object? y)
        {
            return AreEqual(x, y);
        }

        public int GetHashCode(object obj)
        {
            return JsonSerializer.Serialize(obj).GetHashCode();
        }
    }
}

