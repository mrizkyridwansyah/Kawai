﻿
using Kawai.Domain;
//using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System.Text.Json;


[Serializable]
[StructLayout(LayoutKind.Auto)]
public readonly struct EpochDateTime :
    IComparable<EpochDateTime>,
    IComparable<EpochDateTime?>,
    IComparable<long>,
    IEquatable<EpochDateTime>,
    IEquatable<EpochDateTime?>,
    IEquatable<long>
{
    public long Value { get; init; }

    public EpochDateTime()
    {

    }

    public EpochDateTime(long value)
    {
        Value = value;
    }

    public EpochDateTime(int value)
    {
        Value = value;
    }

    public EpochDateTime(string value)
    {
        Value = Convert.ToInt64(value);
    }

    public static implicit operator EpochDateTime(long v) => new EpochDateTime(v);

    public static implicit operator EpochDateTime(int v) => new EpochDateTime(v);

    public static implicit operator long(EpochDateTime v)
    {
        return v.Value;
    }

    public DateTime Date
    {
        get
        {
            var date = new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(Value);
            return date.AddHours(-date.Hour).AddMinutes(-date.Minute).AddSeconds(-date.Second).AddMilliseconds(-date.Millisecond).AddMicroseconds(-date.Microsecond);
        }
    }

    public DateTime DateTime => new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(Value);

    public int Year => Date.Year;

    public int Month => Date.Month;

    public int Day => Date.Day;

    public int Hour => Date.Hour;

    public int Minute => Date.Hour;

    public int Second => Date.Second;

    public EpochDateTime AM120000 => new EpochDateTime(Value).AddHours(-Hour);

    public EpochDateTime PM115959 => AM120000.AddDays(1).AddSeconds(-1);

    public static EpochDateTime Now => new EpochDateTime(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());

    public EpochDateTime AddMicroseconds(long microseconds)
    {
        return Date.AddMicroseconds(microseconds).ToUnixTimeMilliseconds();
    }

    public EpochDateTime AddMilliseconds(long miliseconds)
    {
        return Date.AddMilliseconds(miliseconds).ToUnixTimeMilliseconds();
    }

    public EpochDateTime AddSeconds(int seconds)
    {
        return Date.AddSeconds(seconds).ToUnixTimeMilliseconds();
        //return Value + (seconds * 1000);
    }

    public EpochDateTime AddMinutes(int minutes)
    {
        return Date.AddHours(minutes).ToUnixTimeMilliseconds();
        //return Value + (minutes * 1000 * 60);
    }

    public EpochDateTime AddHours(int hours)
    {
        return Date.AddHours(hours).ToUnixTimeMilliseconds();
        //return Value + (hours * 1000 * 60 * 60);
    }

    public EpochDateTime AddDays(int days)
    {
        return Date.AddDays(days).ToUnixTimeMilliseconds();
        //return Value + (days * 1000 * 60 * 60 * 24);
    }

    public EpochDateTime AddMonths(int months)
    {
        return Date.AddMonths(months).ToUnixTimeMilliseconds();
    }

    public EpochDateTime AddTime(string time)
    {
        var c = time.Split(":");
        if (c.Length != 2)
            throw new ArgumentException("Invalid time format HH:mm");

        var hours = c[0].ToInt32();
        var minutes = c[1].ToInt32();

        return AddHours(hours).AddMinutes(minutes);
    }

    public TimeSpan Subtract(EpochDateTime date)
    {
        return Date.Subtract(date.Date);
    }

    public EpochDateTime ToOffset(int offset)
    {
        if (offset == 0)
            return this;

        return DateTime.AddHours((offset / 60) * -1).ToUnixTimeMilliseconds();
    }

    public int CompareTo(EpochDateTime other)
    {
        return Value.CompareTo(other.Value);
    }

    public int CompareTo(long other)
    {
        return Value.CompareTo(other);
    }

    public bool Equals(EpochDateTime other)
    {
        return Value == other.Value;
    }

    public bool Equals(long other)
    {
        return Value == other;
    }

    public int CompareTo(EpochDateTime? other)
    {
        return Value.CompareTo(other?.Value);
    }

    public bool Equals(EpochDateTime? other)
    {
        return Value == other?.Value;
    }

    public static bool operator ==(EpochDateTime? left, EpochDateTime? right)
    {
        return left?.Value == right?.Value;
    }

    public override int GetHashCode()
    {
        return 0;
    }

    public static bool operator !=(EpochDateTime? left, EpochDateTime? right)
    {
        return left?.Value != right?.Value;
    }

    //public override bool Equals(object obj)
    //{
    //    return Value == obj.ToLong();
    //}

    public static bool operator <(EpochDateTime left, EpochDateTime right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(EpochDateTime left, EpochDateTime right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(EpochDateTime left, EpochDateTime right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(EpochDateTime left, EpochDateTime right)
    {
        return left.CompareTo(right) >= 0;
    }

    #region conversion
    public string ToString(IFormatProvider provider)
    {
        return Date.ToString(provider);
    }

    public string ToString(string format)
    {
        return Date.ToString(format);
    }
    #endregion
}

public class EpochDateTimeConverter : System.Text.Json.Serialization.JsonConverter<EpochDateTime>
{
    public override EpochDateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
            return new EpochDateTime(reader.GetInt64());

        return new EpochDateTime(reader.GetString());
    }

    public override void Write(Utf8JsonWriter writer, EpochDateTime value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Value);
    }
}

//public class NewtonJsonEpochDateTimeConverter : JsonConverter<EpochDateTime>
//{
//    public override EpochDateTime ReadJson(JsonReader reader, Type objectType, EpochDateTime existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
//    {
//        //if (reader.TokenType == JsonToken.Integer)
//        //    return new EpochDateTime(reader.Value.ToLong());

//        return new EpochDateTime(Convert.ToInt64(reader.Value));
//    }

//    public override void WriteJson(JsonWriter writer, EpochDateTime value, Newtonsoft.Json.JsonSerializer serializer)
//    {
//        writer.WriteValue(value.Value);
//    }
//}

//public class NewtonJsonNullableEpochDateTimeConverter : JsonConverter<EpochDateTime?>
//{
//    public override EpochDateTime? ReadJson(JsonReader reader, Type objectType, EpochDateTime? existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
//    {
//        if (reader.TokenType == JsonToken.Null) return null;
//        //if (reader.TokenType == JsonToken.Integer)
//        //    return new EpochDateTime(reader.Value.ToLong());

//        return new EpochDateTime(Convert.ToInt64(reader.Value));
//    }

//    public override void WriteJson(JsonWriter writer, EpochDateTime? value, Newtonsoft.Json.JsonSerializer serializer)
//    {
//        writer.WriteValue(value.Value);
//    }
//}
