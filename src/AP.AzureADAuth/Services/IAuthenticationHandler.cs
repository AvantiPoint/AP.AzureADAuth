using System;

namespace AP.AzureADAuth.Services
{
    public interface IAuthenticationHandler
    {
        IObservable<string> AccessToken { get; }
    }
}
