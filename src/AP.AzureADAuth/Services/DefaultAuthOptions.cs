using System;

namespace AP.AzureADAuth.Services
{
    internal class DefaultAuthOptions : IAuthOptions
    {
        public string Tenant => "microsoft";
        public string Policy { get; }
        public string[] Scopes => new string[] { "User.Read" };
        public string ClientId => $"{default(Guid)}";
        public bool IsB2C { get; }
    }
}
