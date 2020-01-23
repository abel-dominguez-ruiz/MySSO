using MySSO.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MySSO.Application.Configuration
{
    public class DefaultUsersConfig
    {
        public static IEnumerable<(ApplicationUser User, string Password)> Get()
        {
            return new[]
            {
                    (new ApplicationUser
                    {
                        UserName = "guille.puertas",
                        Email = "gillepuertas@doosan.com",
                        FirstName = "Guillermo",
                        LastName = "Puertas",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        EmailConfirmed = true
                    },
                    "123123"),
                    (new ApplicationUser
                    {
                        UserName = "Abelinus",
                        Email = "abeldr1988@gmail.com",
                        FirstName = "Abel",
                        LastName = "Dominguez",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        EmailConfirmed = true
                    },
                    "123123"),
                };
        }

    }
}
