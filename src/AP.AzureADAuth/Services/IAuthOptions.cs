namespace AP.AzureADAuth.Services
{
    public interface IAuthOptions
    {
        string Tenant { get; }
        string Policy { get; }
        string[] Scopes { get; }
        string ClientId { get; }
        bool IsB2C { get; }
    }
}
