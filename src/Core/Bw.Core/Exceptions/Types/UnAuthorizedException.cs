using System.Net;

namespace Bw.Core.Exceptions.Types;

public class UnAuthorizedException : IdentityException
{
    public UnAuthorizedException(string message)
        : base(message, HttpStatusCode.Unauthorized) { }
}
