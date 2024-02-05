using Bw.Core.Exceptions;
using System.Net;

namespace Bw.Core.Exceptions.Types;

public class AppException : CustomException
{
    public AppException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        : base(message)
    {
        StatusCode = statusCode;
    }
}


