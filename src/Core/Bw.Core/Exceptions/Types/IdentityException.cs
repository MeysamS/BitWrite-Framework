using System.Net;

namespace Bw.Core.Exceptions.Types;

public class IdentityException : CustomException
{
    public IdentityException(
        string message,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        params string[] errors
    )
        : base(message, statusCode, errors) { }
}

