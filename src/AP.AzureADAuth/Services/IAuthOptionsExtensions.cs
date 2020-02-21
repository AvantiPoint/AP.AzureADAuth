namespace AP.AzureADAuth.Services
{
    internal static class IAuthOptionsExtensions
    {
        internal static string RedirectUri(this IAuthOptions options)
        {
            return $"msal{options.ClientId}://auth";
        }

        internal static string GetTenantName(this string tenant)
        {
            if (tenant.Split('.').Length == 1)
            {
                tenant = $"{tenant}.onmicrosoft.com";
            }

            return tenant.ToLower();
        }

        internal static string[] GetScopes(this IAuthOptions options)
        {
            if(options.Scopes is null || options.Scopes.Length == 0)
            {
                return new[] { $"{options.Tenant.GetTenantName()}/mobile/read" };
            }

            return options.Scopes;
        }

        internal static string GetPolicy(this IAuthOptions options)
        {
            return string.IsNullOrEmpty(options.Policy) ? "B2C_1_SUSI" : options.Policy;
        }

        internal static string GetB2CAuthority(this IAuthOptions options)
        {
            return $"https://login.microsoftonline.com/tfp/{options.Tenant.GetTenantName()}/{options.GetPolicy()}";
        }
    }
}
