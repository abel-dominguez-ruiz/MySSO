using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

namespace MySSO.EF.Context.Configuration
{
    public class IdentityConfigurationDbContext : ConfigurationDbContext
    {
        public IdentityConfigurationDbContext(
                    DbContextOptions<ConfigurationDbContext> options, 
                    ConfigurationStoreOptions storeOptions) :
            base(options, storeOptions)
        {

        }
    }
}
