using Bw.Core.Exceptions;
using System.Net;

namespace Bw.Domain.Exceptions.Types;

public class DomainException : CustomException
{
    public DomainException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest) : base(message)
    {
        StatusCode = statusCode;
    }
}
