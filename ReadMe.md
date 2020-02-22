# AP.AzureADAuth

The Azure Active Directory Auth Module is a Prism Module for Xamarin Forms projects. This module will enable your application to easily add either Azure Active Directory or Azure Active Directory B2C authentication to your applicaiton by installing this module and providing your own instance of IAuthOptions.

## Using the Module

This module can be installed via NuGet. It has a dependency on using the [Prism.Container.Extensions](https://github.com/dansiegel/Prism.Container.Extensions). It's recommended that you do not use Prism.Unity.Forms or Prism.DryIoc.Forms but rather that you use the Extensions package for the container you want with Prism.Forms.Extended. Alternatively you can use the extensions package with Prism.Forms. To do that you would need to modify your PrismApplication to inherit from PrismApplicationBase and implement `CreateContainerExtension` as follows:

```cs
protected override IContainerExtension CreateContainerExtension() => PrismContainerExtension.Current;
```

> NOTE: If using Prism.Forms.Extended, the Container is automatically picked up and you do not need to modify anything at all from your existing Prism Application.

## Configuring The Module

This module will intelligently contruct the Microsoft Identity Client allowing you to specify a bare minimum of configuration information. You can easily configure your app to use Azure Active Directory B2C by implementing and registering IB2COptions. This will assume a default scope like `https://contoso.onmicrosoft.com/mobile/read` with a Policy of `B2C_1_SUSI`.

```cs
public class B2COptions : IB2COptions
{
    // This should be the Tenant Name (i.e. contoso) not the FQDN (i.e. contoso.onmicrosoft.com)
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
    public string Tenant => Secrets.TenantName;
    public string Policy => "B2C_1_SUSI";
    public string[] Scopes => new[] { $"https://{Secrets.TenantName}.onmicrosoft.com/mobile/read" };
    public string ClientId => Secrets.ClientId;
    public bool IsB2C => true;
}
```

## Customizing the LoginPage

The Login Page is extremely customizable, and can be swapped out completely for your own custom Page.

### Logo

By Default the Login Page will display the AvantiPoint logo above a login button. You can replace this with your own custom logo in any of the following 3 ways:

- Embed a file named `Logo.png` in the same assembly as your Application
- Add a Resource to your Application named LoginLogo
  - Add a String with the local resource name of a natively bundled image asset
  - Add any ImageSource

### Theme

The Login Page uses a special Markup Extension that will attempt to provide a named color from the Application Resources. This will work whether the color has the `Color` suffix or not.

- Accent | AccentColor
- NavigationText | NavigationTextColor

## Using your own LoginPage

The AzureADAuthModule is provided with and without a Generic ContentPage parameter. If you want to provide your own custom LoginPage there are two properties to be aware of:

- LoginCommand
- IsBusy

As you might imagine the IsBusy property will be true whenever the LoginCommand is executing. You can bind to either of these two properties and simply register the module as follows:

```cs
moduleCatalog.AddModule<AzureADAuthModule<MyCustomLoginPage>>();
```