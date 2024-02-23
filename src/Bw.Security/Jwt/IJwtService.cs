using System.Security.Claims;

namespace Bw.Security.Jwt;

public interface IJwtService
{
    GenerateTokenResult GenerateJwtToken(
        string userName,
        string email,
        string userId,
        bool? isVerified = null,
        string? fullName = null,
        string? refreshToken = null,
        IReadOnlyList<Claim>? usersClaims = null,
        IReadOnlyList<string>? rolesClaims = null,
        IReadOnlyList<string>? permissionsClaims = null
    );

    ClaimsPrincipal? GetPrincipalFromToken(string token);
}
