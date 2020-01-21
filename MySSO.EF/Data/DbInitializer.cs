using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySSO.EF.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySSO.EF.Data
{
    public class DbInitializer
    {
        public static async Task InitializeDatabase(
            IServiceProvider services,
            IEnumerable<IdentityResource> identityResources,
            IEnumerable<ApiResource> apiResources,
            IEnumerable<Client> clients)
        {
            var configurationDbContext = services.GetRequiredService<ConfigurationDbContext>();
            var persistedGrantDbContext = services.GetRequiredService<PersistedGrantDbContext>();
            var applicationDbContext = services.GetRequiredService<IdentityServerDbContext>();

            var dbContexts = new List<DbContext>
            {
                configurationDbContext,
                persistedGrantDbContext,
                applicationDbContext
            };

            dbContexts.ForEach(c => c.Database.Migrate());

            //try
            //{
            //    await InitializeConfigurationDatabaseAsync(configurationDbContext, identityResources, apiResources, clients);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //    throw;
            //}
        }

        private static async Task InitializeConfigurationDatabaseAsync(
            ConfigurationDbContext configurationDbContext,
            IEnumerable<IdentityResource> identityResources,
            IEnumerable<ApiResource> apiResources,
            IEnumerable<Client> clients)
        {
            var identityResourceNames = identityResources.Select(x => x.Name).ToArray();
            var existingIdentityResources = await configurationDbContext.IdentityResources.Where(x => identityResourceNames.Contains(x.Name)).ToListAsync();
            foreach (var identityResource in identityResources.Where(x => !existingIdentityResources.Any(y => y.Name == x.Name)))
            {
                configurationDbContext.IdentityResources.Add(identityResource);
            }

            var apiResourceNames = apiResources.Select(x => x.Name).ToArray();
            var existingApiResources = await configurationDbContext.ApiResources.Where(x => apiResourceNames.Contains(x.Name)).ToListAsync();
            foreach (var apiResource in apiResources.Where(x => !existingApiResources.Any(y => y.Name == x.Name)))
            {
                configurationDbContext.ApiResources.Add(apiResource);
            }

            var clientIds = clients.Select(x => x.ClientId).ToArray();
            var existingClientIds = await configurationDbContext.Clients.Where(x => clientIds.Contains(x.ClientId)).Select(x => x.ClientId).ToListAsync();
            var existingClients = new List<Client>();
            foreach (var id in existingClientIds)
            {
                existingClients.Add(await FindClientByIdAsync(configurationDbContext, id));
            }


            foreach (var client in clients)
            {
                var c = existingClients.FirstOrDefault(x => x.ClientId == client.ClientId) ?? client;
                if (c != client)
                {
                    var newRedirectUris = client.RedirectUris.Where(x => !c.RedirectUris.Any(y => y.RedirectUri == x.RedirectUri));
                    if (newRedirectUris.Any()) c.RedirectUris.AddRange(newRedirectUris.Select(x => new ClientRedirectUri { RedirectUri = x.RedirectUri }));

                    var newAllowedCorsOrigins = client.AllowedCorsOrigins.Where(x => !c.AllowedCorsOrigins.Any(y => y.Origin == x.Origin));
                    if (newAllowedCorsOrigins.Any()) c.AllowedCorsOrigins.AddRange(newAllowedCorsOrigins.Select(x => new ClientCorsOrigin { Origin = x.Origin }));

                    var newPostLogoutRedirectUris = client.PostLogoutRedirectUris.Where(x => !c.PostLogoutRedirectUris.Any(y => y.PostLogoutRedirectUri == x.PostLogoutRedirectUri));
                    if (newPostLogoutRedirectUris.Any()) c.PostLogoutRedirectUris.AddRange(newPostLogoutRedirectUris.Select(x => new ClientPostLogoutRedirectUri { PostLogoutRedirectUri = x.PostLogoutRedirectUri }));

                    var newClientSecrets = client.ClientSecrets.Where(x => !c.ClientSecrets.Any(y => y.Value == x.Value)).ToList();
                    if (newClientSecrets.Any())
                    {
                        for (var i = 0; i < newClientSecrets.Count; i++)
                        {
                            c.ClientSecrets.Add(new ClientSecret { Description = $"Auto {DateTime.UtcNow.ToString("yyyyMMdd ")}-{i + 1}", Value = newClientSecrets[i].Value, Expiration = newClientSecrets[i].Expiration });
                        }
                    }

                    var allowedScopes = client.AllowedScopes.Where(x => !c.AllowedScopes.Any(y => y.Scope == x.Scope));
                    if (allowedScopes.Any()) c.AllowedScopes.AddRange(allowedScopes.Select(x => new ClientScope { Scope = x.Scope }));

                    c.AllowedGrantTypes.Clear();
                    c.AllowedGrantTypes.AddRange(client.AllowedGrantTypes.Select(x => new ClientGrantType { GrantType = x.GrantType }));

                    c.Properties.Clear();
                    c.Properties.AddRange(client.Properties.Select(x => new ClientProperty { Key = x.Key, Value = x.Value }));

                    c.Claims.Clear();
                    c.Claims.AddRange(client.Claims.Select(x => new ClientClaim { Type = x.Type, Value = x.Value }));

                    c.Enabled = client.Enabled;
                    c.ProtocolType = client.ProtocolType;
                    c.RequireClientSecret = client.RequireClientSecret;
                    c.ClientName = client.ClientName;
                    c.Description = client.Description;
                    c.ClientUri = client.ClientUri;
                    c.LogoUri = client.LogoUri;
                    c.RequireConsent = client.RequireConsent;
                    c.AllowRememberConsent = client.AllowRememberConsent;
                    c.AlwaysIncludeUserClaimsInIdToken = client.AlwaysIncludeUserClaimsInIdToken;
                    c.RequirePkce = client.RequirePkce;
                    c.AllowPlainTextPkce = client.AllowPlainTextPkce;
                    c.AllowAccessTokensViaBrowser = client.AllowAccessTokensViaBrowser;
                    c.FrontChannelLogoutUri = client.FrontChannelLogoutUri;
                    c.FrontChannelLogoutSessionRequired = client.FrontChannelLogoutSessionRequired;
                    c.BackChannelLogoutUri = client.BackChannelLogoutUri;
                    c.BackChannelLogoutSessionRequired = client.BackChannelLogoutSessionRequired;
                    c.AllowOfflineAccess = client.AllowOfflineAccess;
                    c.IdentityTokenLifetime = client.IdentityTokenLifetime;
                    c.AccessTokenLifetime = client.AccessTokenLifetime;
                    c.AuthorizationCodeLifetime = client.AuthorizationCodeLifetime;
                    c.ConsentLifetime = client.ConsentLifetime;
                    c.AbsoluteRefreshTokenLifetime = client.AbsoluteRefreshTokenLifetime;
                    c.SlidingRefreshTokenLifetime = client.SlidingRefreshTokenLifetime;
                    c.RefreshTokenUsage = client.RefreshTokenUsage;
                    c.UpdateAccessTokenClaimsOnRefresh = client.UpdateAccessTokenClaimsOnRefresh;
                    c.RefreshTokenExpiration = client.RefreshTokenExpiration;
                    c.AccessTokenType = client.AccessTokenType;
                    c.EnableLocalLogin = client.EnableLocalLogin;
                    c.IdentityProviderRestrictions = client.IdentityProviderRestrictions;
                    c.IncludeJwtId = client.IncludeJwtId;
                    c.AlwaysSendClientClaims = client.AlwaysSendClientClaims;
                    c.ClientClaimsPrefix = client.ClientClaimsPrefix;
                    c.PairWiseSubjectSalt = client.PairWiseSubjectSalt;

                    configurationDbContext.Entry(c).State = EntityState.Modified;
                }
                else
                {
                    configurationDbContext.Clients.Add(c);
                }
            }

            await configurationDbContext.SaveChangesAsync();
        }

        private static async Task<Client> FindClientByIdAsync(ConfigurationDbContext context, string clientId)
        {
            var client = await context.Clients.FirstOrDefaultAsync(x => x.ClientId == clientId);
            if (client == null) return null;

            var granTypes = await context.Set<ClientGrantType>().Where(x => x.ClientId == client.Id).ToListAsync();
            var redirectUris = await context.Set<ClientRedirectUri>().Where(x => x.ClientId == client.Id).ToListAsync();
            var postlogoutUris = await context.Set<ClientPostLogoutRedirectUri>().Where(x => x.ClientId == client.Id).ToListAsync();
            var scopes = await context.Set<ClientScope>().Where(x => x.ClientId == client.Id).ToListAsync();
            var secrets = await context.Set<ClientSecret>().Where(x => x.ClientId == client.Id).ToListAsync();
            var claims = await context.Set<ClientClaim>().Where(x => x.ClientId == client.Id).ToListAsync();
            var idpRestrictions = await context.Set<ClientIdPRestriction>().Where(x => x.ClientId == client.Id).ToListAsync();
            var cors = await context.Set<ClientCorsOrigin>().Where(x => x.ClientId == client.Id).ToListAsync();
            var properties = await context.Set<ClientProperty>().Where(x => x.ClientId == client.Id).ToListAsync();

            client.AllowedGrantTypes = granTypes;
            client.RedirectUris = redirectUris;
            client.PostLogoutRedirectUris = postlogoutUris;
            client.AllowedScopes = scopes;
            client.ClientSecrets = secrets;
            client.Claims = claims;
            client.IdentityProviderRestrictions = idpRestrictions;
            client.AllowedCorsOrigins = cors;
            client.Properties = properties;

            return client;
        }
    }


}
