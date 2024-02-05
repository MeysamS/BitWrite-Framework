using System.Net;

namespace Bw.Core.Exceptions.Types;

public class ApiException : CustomException
{
    public ApiException(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        : base(message)
    {
        StatusCode = statusCode;
    }
}
