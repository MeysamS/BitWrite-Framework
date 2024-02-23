using System.Net;

namespace Bw.Core.Exceptions.Types;

public class ConflictException : CustomException
{
    public ConflictException(string message)
        : base(message)
    {
        StatusCode = HttpStatusCode.Conflict;
    }
}

