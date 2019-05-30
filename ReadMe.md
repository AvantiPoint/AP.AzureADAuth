# AP.AzureADAuth

The Azure Active Directory Auth Module is a Prism Module for Xamarin Forms projects. This module will enable your application to easily add either Azure Active Directory or Azure Active Directory B2C authentication to your applicaiton by installing this module and providing your own instance of IAuthOptions.

This module will intelligently contruct the Microsoft Identity Client allowing you to specify a bare minimum of configuration information. You can easily configure your app to use Azure Active Directory B2C by implementing and registering IB2COptions. This will assume a default scope like `https://contoso.onmicrosoft.com/mobile/read` with a Policy of `B2C_1_SUSI`. 

```cs
public class B2COptions : IB2COptions
{
    // This could be the Tenant Name (i.e. Contoso) or the FQDN (i.e. contoso.onmicrosoft.com)
    public string Tenant => Secrets.TenantName;
    public string ClientId => Secrets.ClientId;
    public LogLevel? LogLevel => Microsoft.Identity.Client.LogLevel.Verbose;
}
```

Similarly you can can register IAADOptions for a minimal configuration in which it will default to a `User.Read` scope.

```cs
public class AADOptions : IAADOptions
{
    // This could be the Tenant Name (i.e. Contoso) or the FQDN (i.e. contoso.onmicrosoft.com)
    public string Tenant => Secrets.TenantName;
    public string ClientId => Secrets.ClientId;
    public LogLevel? LogLevel { get; }
}
```

For scenarios where you require more fine grain control you can implement IAuthOptions to configure custom Policies and Scopes.

```cs
public class AuthOptions : IAuthOptions
{
    public LogLevel? LogLevel { get; }
    public string Tenant => Secrets.TenantName.Contains(".")
        ? Secrets.TenantName.ToLower()
        : $"{Secrets.TenantName.ToLower()}.onmicrosoft.com";
    public string Policy => "B2C_1_SUSI";
    public string[] Scopes => new[] { $"https://{Tenant}/mobile/read" };
    public string ClientId => Secrets.ClientId;
    public bool IsB2C => true;
}
```

## Customization

The Login Page is extremely customizable, and can be swapped out completely for your own custom Page.

### Logo

To swap out the logo shown on the Login Page, simply provide an embedded resource named Logo.png in the same project as your PrismApplication.

### Colors

The Login Page uses a special Markup Extension that will attempt to provide a named color from the Application Resources. This will work whether the color has the `Color` suffix or not.

- Accent | AccentColor
- NavigationText | NavigationTextColor

### LoginPage

The AzureADAuthModule is provided with and without a Generic ContentPage parameter. If you want to provide your own custom LoginPage there are two properties to be aware of:

- LoginCommand
- IsBusy

As you might imagine the IsBusy property will be true whenever the LoginCommand is executing. You can bind to either of these two properties and simply register the module as follows:

```cs
moduleCatalog.AddModule<AzureADAuthModule<MyCustomLoginPage>>();
```