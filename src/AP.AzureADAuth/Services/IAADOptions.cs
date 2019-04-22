using Microsoft.Identity.Client;

namespace AP.AzureADAuth.Services
{
    public interface IAADOptions
    {
        string Tenant { get; }
        string ClientId { get; }
        LogLevel? LogLevel { get; }
    }
}
