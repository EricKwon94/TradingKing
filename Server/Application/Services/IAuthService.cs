namespace Application.Services;

public interface IAuthService
{
    string CreateToken(string identifier, string issKey, string iss, string aud);
}
