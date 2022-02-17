using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.IdentityServer4.Validation
{
    internal class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            bool flag = true;
            if (flag)
            {
                var identity = GetIdentityPrincipal("test", "test", "Bearer", DateTime.Now, "ars");
                context.Result = new GrantValidationResult(new ClaimsPrincipal(identity));
            }
            else
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidClient, "error");
            }

            return Task.CompletedTask;
        }

        private static ClaimsPrincipal GetIdentityPrincipal(
            string subject,
            string userrole,
            string authenticationMethod,
            DateTime authTime,
            string identityProvider)
        {
            var source = new List<Claim>()
            {
                new Claim("sub", subject),
                new Claim(ClaimTypes.Role, userrole),
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
