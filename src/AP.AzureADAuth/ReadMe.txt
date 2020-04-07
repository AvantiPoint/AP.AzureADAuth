Getting started with the AP.AzureADAuth module couldn't be easier...

Step 1)
Setup your Active Directory or Active Directory B2C Tenant and Application in the Azure Portal. Be sure to read the docs at https://github.com/AvantiPoint/AP.AzureADAuth for more info

Step 2) Configuring the Platform

On iOS, you will need to update the Info.plist

<key>CFBundleURLTypes</key>
<array>
<dict>
  <key>CFBundleTypeRole</key>
  <string>Editor</string>
  <key>CFBundleURLName</key>
  <string>com.yourcompany.appid</string>
  <key>CFBundleURLSchemes</key>
  <array>
    <string>msal$AzureADClientId$</string>
  </array>
</dict>
</array>

Also be sure to have updated your Entitlements.plist and enable Key Chain access groups

<key>keychain-access-groups</key>
<array>
  <string>$(AppIdentifierPrefix)com.yourcompany.appid</string>
</array>

On Android you will want to add a new activity as follows

[Activity]
[IntentFilter(new[] { Intent.ActionView },
    Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault },
    DataHost = "auth",
    DataScheme = "msal$AzureADClientId$")]
public class MsalActivity : BrowserTabActivity
{

}

Step 3)
Create an options class with your configuration settings for example:

**NOTE** This will use the Policy B2C_1_SUSI and scope https://{tenantName}.onmicrosoft.com/mobile/read
public class B2COptions : IB2COptions
{
    // This should be the Tenant Name (i.e. contoso) not the FQDN (i.e. contoso.onmicrosoft.com)
    public string Tenant => Secrets.TenantName;
    public string ClientId => Secrets.ClientId;
    public LogLevel? LogLevel => Microsoft.Identity.Client.LogLevel.Verbose;
}

Step 4)
Update the registrations in your Prism Application

protected override void RegisterTypes(IContainerRegistry containerRegistry)
{
    containerRegistry.RegisterSingleton<IB2COptions, B2COptions>();
}

protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
{
    moduleCatalog.AddModule<AzureADAuthModule>();
}

Step 5)
Hook into Authentication Events and Navigation to Login Page in your Prism Application

protected override async void OnInitialized()
{
    InitializeComponent();

    var ea = Container.Resolve<IEventAggregator>();
    ea.GetEvent<UserAuthenticatedEvent>().Subscribe(OnUserAuthenticatedEventPublished);
    ea.GetEvent<UserLoggedOutEvent>().Subscribe(OnUserLoggedOutEventPublished);

    var result = await NavigationService.NavigateAsync("login");

    if (!result.Success)
    {
        // Handle Navigation Error
    }
}

Optional Steps)
- Customize the Logo & Theme Colors or use your own custom LoginPage... see https://github.com/AvantiPoint/AP.AzureADAuth
