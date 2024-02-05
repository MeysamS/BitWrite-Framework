using System.Net;

namespace Bw.Core.Exceptions;

public class CustomException : Exception
{
    public IEnumerable<string> ErrorMessages { get; protected set; }

    public HttpStatusCode StatusCode { get; protected set; }


    public CustomException(
    string message,
    HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
    params string[] errors) : base(message)
    {
        ErrorMessages = errors;
        StatusCode = statusCode;
    }
}
