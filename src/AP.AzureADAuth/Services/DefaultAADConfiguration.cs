using Microsoft.Identity.Client;

namespace AP.AzureADAuth.Services
{
    internal class DefaultAADConfiguration : IAuthConfiguration
    {
        private IAADOptions _options { get; }

        public DefaultAADConfiguration(IAADOptions options)
        {
            _options = options;
        }

        public string Authority => $"https://login.microsoftonline.com/tfp/{_options.Tenant.GetFullyQualifiedTenantName()}/{Policy}";
        public string Policy => string.Empty;
        public string[] Scopes => new[] { $"User.Read" };
        public string ClientId => _options.ClientId;
        public string RedirectUri => $"msal{_options.ClientId}://auth";
        public LogLevel? LogLevel => _options.LogLevel;
    }


}
