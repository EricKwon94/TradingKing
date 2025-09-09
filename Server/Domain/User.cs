using System.Text.RegularExpressions;

namespace Domain;

internal class User
{
    public bool Register(string email)
    {
        const string pattern = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
        return Regex.IsMatch(email, pattern);
    }
}
