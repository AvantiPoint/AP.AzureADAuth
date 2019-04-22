using Microsoft.Identity.Client;

namespace AP.AzureADAuth.Services
{
    internal class UserDefinedConfiguration : IAuthConfiguration
    {
        private IAuthOptions _options { get; }

        public UserDefinedConfiguration(IAuthOptions options)
        {
            _options = options;
        }

        public string Authority => $"https://login.microsoftonline.com/tfp/{_options.Tenant.GetTenantName()}/{Policy}";
        public string Policy => _options.Policy;
        public string[] Scopes => _options.Scopes;
        public string ClientId => _options.ClientId;
        public string RedirectUri => $"msal{_options.ClientId}://auth";
        public LogLevel? LogLevel => _options.LogLevel;
    }
}
