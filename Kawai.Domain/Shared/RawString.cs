namespace Kawai.Domain.Shared;

public readonly struct RawString
{
    public string Value { get; }

    public RawString(string value)
    {
        Value = value;
    }

    public override string ToString() => Value;
    public static implicit operator string(RawString rawString) => rawString.Value;
    public static implicit operator RawString(string value) => new RawString(value);
}
