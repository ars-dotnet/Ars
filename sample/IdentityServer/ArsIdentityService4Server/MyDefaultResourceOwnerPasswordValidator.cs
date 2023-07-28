using Ars.Common.Core.AspNetCore;
using IdentityModel;
using IdentityServer4.Validation;
using System.Security.Claims;

namespace ArsIdentityService4Server
{
    public class MyDefaultResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var identity = GetIdentityPrincipal("123","1234","h123", "arsadmin", "Bearer",DateTime.Now,"ars");

            context.Result = new GrantValidationResult(new ClaimsPrincipal(identity));

            return Task.CompletedTask;
        }

        private static ClaimsPrincipal GetIdentityPrincipal(
            string subject,
            string tenant,
            string username,
            string userrole,
            string authenticationMethod,
            DateTime authTime,
            string identityProvider)
        {
            var source = new List<Claim>()
            {
                new Claim(ArsClaimTypes.UserId, subject),
                new Claim(ArsClaimTypes.TenantId, tenant),
                new Claim(ArsClaimTypes.Role, userrole),
                new Claim(ArsClaimTypes.UserName, username),
                new Claim("amr", authenticationMethod),
                new Claim("idp", identityProvider),
                new Claim("auth_time", DateTimeExtensions.ToEpochTime(authTime).ToString(),
                    "http://www.w3.org/2001/XMLSchema#integer")
            };
            var claimsIdentity = new ClaimsIdentity(authenticationMethod);
            claimsIdentity.AddClaims(source.Distinct<Claim>((IEqualityComparer<Claim>)new ClaimComparer()));
            return new ClaimsPrincipal(claimsIdentity);
        }
    }
}
