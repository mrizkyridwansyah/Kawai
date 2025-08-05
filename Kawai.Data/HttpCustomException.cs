namespace Kawai.Data;

public class HttpCustomException : Exception
{
    public int StatusCode { get; }

    public HttpCustomException(int statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }
}
