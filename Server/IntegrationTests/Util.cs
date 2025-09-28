using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace IntegrationTests;

internal static class Util
{
    public static StringContent ToContent(this object o)
    {
        string json = JsonSerializer.Serialize(o);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
