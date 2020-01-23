using IdentityServer4.Models;
using System.Collections.Generic;

namespace MySSO.Application.Configuration
{
    public static class DefaultIdentityResources
    {
        public static IEnumerable<IdentityResource> Get()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResources.Profile(),
            };
        }
    }
}
