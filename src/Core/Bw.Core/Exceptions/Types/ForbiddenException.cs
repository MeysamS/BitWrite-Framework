using System.Net;

namespace Bw.Core.Exceptions.Types;

public class ForbiddenException : IdentityException
{
    public ForbiddenException(string message)
        : base(message, statusCode: HttpStatusCode.Forbidden) { }
}
