using Microsoft.Identity.Client;

namespace AP.AzureADAuth.Services
{
    public interface IB2COptions
    {
        string Tenant { get; }
        string ClientId { get; }
        LogLevel? LogLevel { get; }
    }
}
