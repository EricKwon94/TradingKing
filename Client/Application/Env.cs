using System;

namespace Application;

internal class Env
{
#if DEBUG
    public static readonly Uri serverAddress = new Uri("https://localhost:7073");
#else
    public static readonly Uri serverAddress = new Uri("https://tradingking-dev-koreacentral-01.azurewebsites.net");
#endif
}
