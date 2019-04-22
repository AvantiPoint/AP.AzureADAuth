using AP.AzureADAuth.Services;
using Microsoft.Identity.Client;

namespace ShellApp.Helpers
{
    public class B2COptions : IB2COptions
    {
        public string Tenant => Secrets.Tenant;
        public string ClientId => Secrets.ClientId;
        public LogLevel? LogLevel => Microsoft.Identity.Client.LogLevel.Verbose;
    }
}
