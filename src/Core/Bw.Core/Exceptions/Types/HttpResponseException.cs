using Bw.Core.Exceptions;
using System.Net;

namespace Bw.Core.Exceptions.Types;

public class HttpResponseException : CustomException
{
    public HttpResponseException(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        : base(message, statusCode)
    {
    }
}
