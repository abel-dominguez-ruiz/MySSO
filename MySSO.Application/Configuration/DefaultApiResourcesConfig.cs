

using IdentityModel;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace MySSO.Application.Configuration
{
    public class DefaultApiResourcesConfig
    {
        public static IEnumerable<ApiResource> Get()
        {
            return new[]
            {
                new ApiResource
                {
                    Name = "api",
                    ApiSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    Scopes =
                    {
                        new Scope()
                        {
                            Name = "api.full_access",
                            DisplayName = "Full access to API",
                        },
                        new Scope
                        {
                            Name = "api.read_only",
                            DisplayName = "Read only access to API"
                        },
                        new Scope
                        {
                            Name = "api.external_client",
                            DisplayName = "Read/Write only supplier"
                        }
                    },
                    UserClaims = {JwtClaimTypes.Name, JwtClaimTypes.Email}
                }
            };
        }

    }
}
