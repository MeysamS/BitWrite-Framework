namespace Bw.Security.Jwt;

public record GenerateTokenResult(string AccessToken, DateTime ExpireAt);
