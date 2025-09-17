namespace Application;

public interface IAuthService
{
    string CreateToken(string identifier, string issKey, string iss, string aud);
}
