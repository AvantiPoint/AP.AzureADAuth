using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace AP.AzureADAuth.Services
{
    internal interface IAuthenticationService
    {
        Task<AuthenticationResult> LoginAsync();

        Task<AuthenticationResult> LoginSilentAsync();

        Task LogoutAsync();
    }
}
