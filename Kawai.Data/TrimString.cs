using Dapper;
using System.Data;

namespace Kawai.Domain.Shared;

public class TrimString : SqlMapper.TypeHandler<string>
{
    public override string? Parse(object value)
    {
        return value?.ToString()?.Trim() ?? string.Empty;
    }
    public override void SetValue(IDbDataParameter parameter, string value)
    {
        parameter.Value = value?.ToString()?.Trim() ?? string.Empty;
    }
}
