using Bw.Core.Exceptions;
using System.Net;

namespace Bw.Core.Exceptions.Types;

public class NotFoundException : CustomException
{
    public NotFoundException(string message)
        : base(message)
    {
        StatusCode = HttpStatusCode.NotFound;
    }
}

