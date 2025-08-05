namespace Kawai.Domain.Shared;

public class DataResult<T>
{
    public List<T> Items { get; set; }
    public int Length { get; set; }
    public int Page { get; set; }
    public int Total { get; set; }
    public int Filtered { get; set; }
}
