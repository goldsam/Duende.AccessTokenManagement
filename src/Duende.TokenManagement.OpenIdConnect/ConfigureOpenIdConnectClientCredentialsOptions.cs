// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Duende.TokenManagement.ClientCredentials;
using Microsoft.Extensions.Options;

namespace Duende.TokenManagement.OpenIdConnect;

/// <summary>
/// Named options to synthetize client credentials based on OIDC handler configuration
/// </summary>
public class ConfigureOpenIdConnectClientCredentialsOptions : IConfigureNamedOptions<ClientCredentialsClient>
{
    private readonly IOpenIdConnectConfigurationService _configurationService;
    private readonly UserAccessTokenManagementOptions _options;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="configurationService"></param>
    /// <param name="options"></param>
    public ConfigureOpenIdConnectClientCredentialsOptions(
        IOpenIdConnectConfigurationService configurationService,
        IOptions<UserAccessTokenManagementOptions> options)
    {
        _configurationService = configurationService;
        _options = options.Value;
    }

    /// <inheritdoc />
    public void Configure(ClientCredentialsClient options)
    { }

    /// <inheritdoc />
    public void Configure(string name, ClientCredentialsClient options)
    {
        if (!name.StartsWith(OpenIdConnectTokenManagementDefaults.ClientCredentialsClientNamePrefix)) return;
        
        string? scheme = null;
        if (name.Length > OpenIdConnectTokenManagementDefaults.ClientCredentialsClientNamePrefix.Length)
        {
            scheme = name[OpenIdConnectTokenManagementDefaults.ClientCredentialsClientNamePrefix.Length..];
        }
            
        // todo: how to async?
        var oidc = _configurationService.GetOpenIdConnectConfigurationAsync(scheme).GetAwaiter().GetResult();
            
        options.Address = oidc.TokenEndpoint;
        options.ClientId = oidc.ClientId;
        options.ClientSecret = oidc.ClientSecret;
        options.ClientCredentialStyle = _options.ClientCredentialStyle;
        options.Scope = _options.ClientCredentialsScope;
        options.Resource = _options.ClientCredentialsResource;
        options.HttpClient = oidc.HttpClient;
    }
}