# AP.AzureADAuth

The Azure Active Directory Auth Module is a Prism Module for Xamarin Forms projects. This module will enable your application to easily add either Azure Active Directory or Azure Active Directory B2C authentication to your applicaiton by installing this module and providing your own instance of IAuthOptions. 

```cs
public class AuthOptions : IAuthOptions
{
    public string Tenant => Secrets.Tenant;
    public string Policy => Secrets.Policy;
    public string[] Scopes => Secrets.Scopes.Split(',');
    public string ClientId => Secrets.ClientId;
    public bool IsB2C => true;
}
```
