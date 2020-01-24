using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySSO.EF.Context;
using MySSO.Entity.Entities;
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
            IEnumerable<Client> clients,
            IEnumerable<(ApplicationUser User, string Password)> applicationUsers)
        {
            var configurationDbContext = services.GetRequiredService<ConfigurationDbContext>();
            var persistedGrantDbContext = services.GetRequiredService<PersistedGrantDbContext>();
            var applicationDbContext = services.GetRequiredService<IdentityServerDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            var dbContexts = new List<DbContext>
            {
                configurationDbContext,
                persistedGrantDbContext,
                applicationDbContext
            };

            dbContexts.ForEach(c => c.Database.Migrate());

            try
            {
                await InitializeConfigurationDatabaseAsync(
                    configurationDbContext,
                    userManager, 
                    identityResources, 
                    apiResources, 
                    clients, applicationUsers);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static async Task InitializeConfigurationDatabaseAsync(
            ConfigurationDbContext configurationDbContext,
            UserManager<ApplicationUser> userManager,
            IEnumerable<IdentityResource> identityResources,
            IEnumerable<ApiResource> apiResources,
            IEnumerable<Client> clients,
            IEnumerable<(ApplicationUser User, string Password)> applicationUsers)
        {
            foreach ((var user, var password) in applicationUsers)
            {
                var item = await userManager.FindByNameAsync(user.UserName);
                if (item == null)
                {
                    await userManager.CreateAsync(user, password);
                }
            }

            //TODO: We should consider if a condition is necessary. Discuss
            foreach (var client in clients)
            {
                var item = await configurationDbContext.Clients.FirstOrDefaultAsync(c => c.ClientId == client.ClientId);
                if (item != null)
                {
                    configurationDbContext.Clients.Remove(item);
                }
                configurationDbContext.Clients.Add(client);
            }

            foreach (var apiResource in apiResources)
            {
                var item = await configurationDbContext.ApiResources.FirstOrDefaultAsync(c => c.Name == apiResource.Name);
                if (item != null)
                {
                    configurationDbContext.ApiResources.Remove(item);
                }
                configurationDbContext.ApiResources.Add(apiResource);
            }

            if (!configurationDbContext.IdentityResources.Any())
            {
                foreach (var resource in identityResources)
                {
                    configurationDbContext.IdentityResources.Add(resource);
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
