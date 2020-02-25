using Microsoft.Identity.Client;

namespace AP.AzureADAuth.Services
{
    internal class DefaultB2CConfiguration : IAuthConfiguration
    {
        private IB2COptions _options { get; }

        public DefaultB2CConfiguration(IB2COptions options)
        {
            _options = options;
        }

        public string Authority => $"https://{_options.Tenant.GetTenantName()}.b2clogin.com/tfp/{_options.Tenant.GetFullyQualifiedTenantName()}/{Policy}";
        public string Policy => "B2C_1_SUSI";
        public string[] Scopes => new[] { $"https://{_options.Tenant.GetFullyQualifiedTenantName()}/mobile/read" };
        public string ClientId => _options.ClientId;
        public string RedirectUri => $"msal{_options.ClientId}://auth";
        public LogLevel? LogLevel => _options.LogLevel;
    }


}
