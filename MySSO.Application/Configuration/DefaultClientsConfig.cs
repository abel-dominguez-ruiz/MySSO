using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace MySSO.Application.Configuration
{
    public class DefaultClientsConfig
    {
        public static IEnumerable<Client> Get(IConfiguration configuration)
        {
            var frontendBaseUris = configuration
                .GetSection("DefaultClients:RedirectFrontendUrls")
                .Get<string[]>()
                .Select(baseUri => new Uri(baseUri))
                .ToList();
            return new List<Client>
            {
                new Client
                {
                    ClientId = "sso_postman",
                    ClientName = "Postman Client Dev",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets =
                    {
                        new Secret("yabadabadoo-hypercompuglobalmeganet".Sha256())
                    },
                    RequireConsent = false,
                    RequireClientSecret = true,
                    AllowOfflineAccess = true,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "api.full_access"
                    },
                },
                new Client
                {
                    ClientId = "front-end-client",
                    ClientName = "Frontend",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    RedirectUris = frontendBaseUris
                        .SelectMany(baseUri => new [] {
                            new Uri(baseUri, "auth.html").AbsoluteUri,
                            new Uri(baseUri, "silent-renew.html").AbsoluteUri
                        })
                        .ToList(),
                    PostLogoutRedirectUris = frontendBaseUris
                        .Select(baseUri => new Uri(baseUri, "logout.html").AbsoluteUri)
                        .ToList(),
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "api.full_access"
                    },
                    AllowOfflineAccess = true,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false
                },
                    new Client
                {
                    ClientId = "client-credential-client",
                    ClientName = "Client Credential",
                     ClientSecrets =
                    {
                        new Secret("client-test-secret".Sha256())
                    },
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes =
                    {
                        "api.external_client"
                    },
                    AllowOfflineAccess = true,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false
                }
            };
        }

    }
}
