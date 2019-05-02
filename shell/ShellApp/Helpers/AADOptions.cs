using AP.AzureADAuth.Services;
using Microsoft.Identity.Client;

namespace ShellApp.Helpers
{
    public class AADOptions : IAADOptions
    {
        public string Tenant => Secrets.TenantName;
        public string ClientId => Secrets.ClientId;
        public LogLevel? LogLevel { get; }
    }
}
