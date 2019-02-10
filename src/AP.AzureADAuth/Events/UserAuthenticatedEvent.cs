using Microsoft.Identity.Client;
using Prism.Events;

namespace AP.AzureADAuth.Events
{
    public class UserAuthenticatedEvent : PubSubEvent<AuthenticationResult>
    {
    }
}
