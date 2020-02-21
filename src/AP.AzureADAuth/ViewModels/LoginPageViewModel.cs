using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AP.AzureADAuth.Events;
using AP.AzureADAuth.Services;
using Microsoft.Identity.Client;
using Prism.AppModel;
using Prism.Events;
using Prism.Logging;
using Prism.Navigation;
using ReactiveUI;

namespace AP.AzureADAuth.ViewModels
{
    internal class LoginPageViewModel : ReactiveObject, IPageLifecycleAware, IDestructible
    {
        private IAuthenticationService _authenticationService { get; }
        private IEventAggregator _eventAggregator { get; }
        private ILogger _logger { get; }

        public LoginPageViewModel(IAuthenticationService authenticationService, IEventAggregator eventAggregator, ILogger logger)
        {
            _authenticationService = authenticationService;
            _eventAggregator = eventAggregator;
            _logger = logger;

            LoginCommand = ReactiveCommand.CreateFromTask(OnLoginCommandExecuted,
                this.WhenAnyValue(x => x.IsBusy)
                .Select(x => !x));
            _isBusyHelper = this.WhenAnyObservable(x => x.LoginCommand.IsExecuting)
                .ToProperty(this, x => x.IsBusy, false);
        }

        private ObservableAsPropertyHelper<bool> _isBusyHelper;
        public bool IsBusy => _isBusyHelper?.Value ?? false;

        public ReactiveCommand<Unit, Unit> LoginCommand { get; set; }

        public async void OnAppearing()
        {
            _logger.TrackEvent("Login Page");

            if (await LoginCommand.CanExecute.FirstAsync())
            {
                await LoginCommand.Execute();
            }
        }

        public void OnDisappearing() { }

        public void Destroy()
        {
            _isBusyHelper.Dispose();
            _isBusyHelper = null;
        }

        private async Task OnLoginCommandExecuted()
        {
            try
            {
                var result = await _authenticationService.LoginAsync();

                if (!(result is null))
                {
                    _eventAggregator.GetEvent<UserAuthenticatedEvent>().Publish(result);
                }
            }
            catch (Exception ex)
            {
                var data = new Dictionary<string, string>
                {
                    { "page", "Login" }
                };

                if(ex is MsalException msal)
                {
                    data.Add("errorCode", msal.ErrorCode);
                    if (msal.ErrorCode == MsalError.AuthenticationCanceledError)
                    {
                        _logger.TrackEvent("User Canceled Login");
                        return;
                    }
                }

#if DEBUG
                System.Diagnostics.Debugger.Break();
#endif
                _logger.Report(ex, data);
            }
        }
    }
}
