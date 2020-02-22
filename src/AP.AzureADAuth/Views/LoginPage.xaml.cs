using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AP.AzureADAuth.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage
    {
        public LoginPage()
        {
            InitializeComponent();

            var app = Application.Current;

            if(app != null && app.Resources.ContainsKey("LoginPageBackground"))
            {
                switch(app.Resources["LoginPageBackground"])
                {
                    case ImageSource imageSource:
                        BackgroundImageSource = imageSource;
                        break;
                    case View view:
                        backgroundContainer.Content = view;
                        break;
                }
            }
        }
    }
}