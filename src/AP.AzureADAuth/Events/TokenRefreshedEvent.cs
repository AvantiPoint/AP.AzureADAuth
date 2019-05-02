using Microsoft.Identity.Client;
using Prism.Events;

namespace AP.AzureADAuth.Events
{
    public class TokenRefreshedEvent : PubSubEvent<AuthenticationResult>
    {
    }
}
