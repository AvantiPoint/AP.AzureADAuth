using AP.AzureADAuth.Services;

namespace ShellApp.Helpers
{
    public class AuthOptions : IAuthOptions
    {
        public string Tenant => Secrets.Tenant;
        public string Policy => Secrets.Policy;
        public string[] Scopes => Secrets.Scopes.Split(',');
        public string ClientId => Secrets.ClientId;
        public bool IsB2C => true;
    }
}
