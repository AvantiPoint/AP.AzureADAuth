using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using Prism.Mvvm;
using Prism.Navigation;

namespace ShellApp.ViewModels
{
    public class MainPageViewModel : BindableBase, IInitialize
    {
        private string _jwtPayload;
        public string JwtPayload
        {
            get => _jwtPayload;
            set => SetProperty(ref _jwtPayload, value);
        }

        public void Initialize(INavigationParameters parameters)
        {
            var result = parameters.GetValue<AuthenticationResult>("authResult");
            var jwt = result.AccessToken.Split('.')[1];
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(jwt));
            JwtPayload = JObject.Parse(json).ToString(Newtonsoft.Json.Formatting.Indented);
        }
    }
}
