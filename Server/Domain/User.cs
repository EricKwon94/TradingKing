using System.Text.RegularExpressions;

namespace Domain;

public class User
{
    public int No { get; set; }
    public string Id { get; set; }
    public string Nickname { get; set; }

    public bool Register(string email)
    {
        const string pattern = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
        return Regex.IsMatch(email, pattern);
    }
}
