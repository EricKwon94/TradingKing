using Application.Services;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Api;

internal class AuthHeaderHandler : DelegatingHandler
{
    private readonly IPreferences _preferences;

    public AuthHeaderHandler(IPreferences preferences)
    {
        _preferences = preferences;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string token = _preferences.Get("jwt", "");

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
