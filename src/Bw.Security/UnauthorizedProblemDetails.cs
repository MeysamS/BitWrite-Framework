using Microsoft.AspNetCore.Mvc;

namespace Bw.Security;

public class UnauthorizedProblemDetails : ProblemDetails
{
    public UnauthorizedProblemDetails(string? details = null)
    {
        Title = "UnauthorizedException";
        Detail = details;
        Status = 401;
        Type = "https://httpstatuses.com/401";
    }
}
