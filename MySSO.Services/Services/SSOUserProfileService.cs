using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using MySSO.EF.Context;
using MySSO.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MySSO.Services.Services
{
    public class SSOUserProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public SSOUserProfileService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await _userManager.FindByIdAsync(context.Subject.GetSubjectId());
            var listClaims = new List<Claim>
                {
                    new Claim(JwtClaimTypes.Email, user.Email, ClaimValueTypes.Email),
                    new Claim(JwtClaimTypes.PreferredUserName , user.UserName, ClaimValueTypes.String),
                    new Claim(JwtClaimTypes.GivenName, user.FirstName, ClaimValueTypes.String),
                    new Claim(JwtClaimTypes.FamilyName, user.LastName, ClaimValueTypes.String),
                    new Claim(JwtClaimTypes.UpdatedAt , user.UpdatedAt.ToUniversalTime().ToString("o"), ClaimValueTypes.DateTime)
            };
            context.IssuedClaims.AddRange(listClaims);
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;

            return Task.FromResult(0);
        }
    }
}
