using System.Net;

namespace Bw.Core.Exceptions.Types;

public class BadRequestException : CustomException
{
    public BadRequestException(string message)
        : base(message)
    {
        StatusCode = HttpStatusCode.NotFound;
    }
}
