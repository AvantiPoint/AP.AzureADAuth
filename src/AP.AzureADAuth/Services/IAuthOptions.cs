﻿using Microsoft.Identity.Client;

namespace AP.AzureADAuth.Services
{
    public interface IAuthOptions
    {
        LogLevel? LogLevel { get; }
        string Tenant { get; }
        string Policy { get; }
        string[] Scopes { get; }
        string ClientId { get; }
        bool IsB2C { get; }
    }
}
