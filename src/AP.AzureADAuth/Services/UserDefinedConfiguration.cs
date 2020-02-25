using Microsoft.Identity.Client;

namespace AP.AzureADAuth.Services
{
    internal class UserDefinedConfiguration : IAuthConfiguration
    {
        private IAuthOptions Options { get; }

        public UserDefinedConfiguration(IAuthOptions options)
        {
            Options = options;
        }

        public string Authority => $"https://login.microsoftonline.com/tfp/{Options.Tenant.GetFullyQualifiedTenantName()}/{Policy}";
        public string Policy => Options.Policy;
        public string[] Scopes => Options.Scopes;
        public string ClientId => Options.ClientId;
        public string RedirectUri => $"msal{Options.ClientId}://auth";
        public LogLevel? LogLevel => Options.LogLevel;
    }
}
