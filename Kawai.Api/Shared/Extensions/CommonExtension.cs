using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Collections;


namespace Kawai.Api.Shared.Extension;

public static class CommonExtension
{
    public static Dictionary<string, string[]> MapErrorMessage(this ModelStateDictionary ModelState)
    {
        return ModelState.Where(x => x.Value.Errors.Count > 0).ToDictionary(kvp => kvp.Key.Replace("$.", ""), kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray());
    }

    public static Dictionary<string, List<string>> TryValidate(this object model, IServiceProvider serviceProvider)
    {
        ValidationContext vc = new(model, serviceProvider, null);
        ICollection<ValidationResult> results = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(model, vc, results, true);

        return results.SelectMany(p => p.MemberNames.Select(c => c)).ToDictionary(p => p.Replace("$.", ""), p => results.Where(c => c.MemberNames.Any(d => d == p)).Select(c => c.ErrorMessage).ToList());
    }

    public static Dictionary<string, List<string>> TryValidateRecursive(this object model, IServiceProvider serviceProvider, string propName = null)
    {
        var list = new Dictionary<string, List<string>>();
        if (model is null)
            return list;

        var enumObjects = model as IEnumerable;
        if (enumObjects != null)
        {
            int i = 0;
            foreach (var enumObj in enumObjects)
            {
                list = list.Concat(TryValidateRecursive(enumObj, serviceProvider, $"{propName}[{i}]")).ToDictionary(x => x.Key, x => x.Value);
                i++;
            }
        }
        else
        {
            var propTypes = new Type[]
            {
                typeof(string), typeof(bool), typeof(bool?),
                typeof(bool), typeof(bool?),
                typeof(int), typeof(int?),
                typeof(long), typeof(long?),
                typeof(double), typeof(double?),
                typeof(byte), typeof(byte?),
                typeof(float), typeof(float?),
                typeof(DateTime), typeof(DateTime?),
                typeof(EpochDateTime), typeof(EpochDateTime?),
            };

            list = TryValidate(model, serviceProvider).ToDictionary(p => $"{(string.IsNullOrWhiteSpace(propName) ? "" : propName + ".")}{p.Key}", p => p.Value);
            var properties = model.GetType().GetProperties().Where(prop => prop.CanRead
                && !propTypes.Contains(prop.PropertyType)
                //&& prop.PropertyType.IsValueType
                && prop.GetIndexParameters().Length == 0).ToList();

            foreach (var property in properties)
            {
                var value = property.GetValue(model);

                if (value == null)
                    continue;

                list = list.Concat(TryValidateRecursive(value, serviceProvider, property.Name)).ToDictionary(x => x.Key, x => x.Value);
            }
        }
        return list;
    }

    public static long ToUnixTimeMilliseconds(this DateTime date)
    {
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        TimeSpan diff = date - origin;
        return (long)diff.TotalMilliseconds;
    }

    public static long ToUnixTimeMilliseconds(this DateTime? date)
    {
        if (!date.HasValue) return 0;
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        TimeSpan diff = date.Value - origin;
        return (long)diff.TotalMilliseconds;
    }

    public static DateTime AddTime(this DateTime date, string time)
    {
        TimeSpan.TryParse(time, out TimeSpan result);
        return date.Add(result);
    }

    public static DateOnly ToDateOnly(this DateTime date)
    {
        return new DateOnly(date.Year, date.Month, date.Day);
    }

    public static DateOnly? ToDateOnly(this DateTime? date)
    {
        if (!date.HasValue) return null;
        return new DateOnly(date.Value.Year, date.Value.Month, date.Value.Day);
    }
}
