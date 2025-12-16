using Kawai.Domain.Shared;

namespace Kawai.Api.Services;

public static class ImportValidator
{
    public static List<string> ValidateAll<T>(List<T> items) where T : ImportBase
    {
        var allErrors = new List<string>();

        foreach (var item in items)
        {
            if (!item.IsValid())
            {
                foreach (var err in item.Errors)
                {
                    allErrors.Add($"Row {item.RowNumber}: {err}");
                }
            }
        }

        return allErrors;
    }
}

public static class ImportableExtension
{
    public static List<Type> Templates { get; set; } = new();
    public static void AddDataImports(this IServiceCollection services)
    {
        Templates = typeof(ImportableAttribute).Assembly.ExportedTypes.Where(p => p.GetCustomAttributes(true).Any(c => c.GetType() == typeof(ImportableAttribute))).ToList();
    }
}
