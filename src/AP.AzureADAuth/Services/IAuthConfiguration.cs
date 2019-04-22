using Microsoft.Identity.Client;

namespace AP.AzureADAuth.Services
{
    internal interface IAuthConfiguration
    {
        string Authority { get; }
        string Policy { get; }
        string[] Scopes { get; }
        string ClientId { get; }
        string RedirectUri { get; }
        LogLevel? LogLevel { get; }
    }
}
