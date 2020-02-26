using System;
using Microsoft.Identity.Client;

namespace AP.AzureADAuth.Services
{
    public interface IAuthenticationHandler
    {
        IObservable<string> AccessToken { get; }

        IObservable<AuthenticationResult> AuthenticationResult { get; }
    }
}
