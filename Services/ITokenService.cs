namespace HttpServer.Services;

public interface ITokenService
{
    string GenerateToken(string id);

    string? GetId(string token);
}