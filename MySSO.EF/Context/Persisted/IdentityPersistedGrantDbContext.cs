using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
namespace MySSO.EF.Context.Persisted
{
    public class IdentityPersistedGrantDbContext : PersistedGrantDbContext
    {
        public IdentityPersistedGrantDbContext(
            DbContextOptions<PersistedGrantDbContext> options,
            OperationalStoreOptions storeOptions) 
            : base(options, storeOptions)
        {
        }
        
    }
}
