using AP.AzureADAuth.Services;
using Microsoft.Identity.Client;

namespace ShellApp.Helpers
{
    public class AuthOptions : IAuthOptions
    {
        public LogLevel? LogLevel { get; }
        public string Tenant => Secrets.TenantName.Contains(".")
            ? Secrets.TenantName.ToLower()
            : $"{Secrets.TenantName.ToLower()}.onmicrosoft.com";
        public string Policy => "B2C_1_SUSI";
        public string[] Scopes => new[] { $"https://{Tenant}/mobile/read" };
        public string ClientId => Secrets.ClientId;
        public bool IsB2C => true;
    }
}
