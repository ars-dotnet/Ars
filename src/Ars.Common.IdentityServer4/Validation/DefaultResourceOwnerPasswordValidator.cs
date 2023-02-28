using Ars.Common.Core.AspNetCore;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Test;
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
    internal class DefaultResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly TestUserStore _users;
        public DefaultResourceOwnerPasswordValidator(TestUserStore users)
        {
            _users = users;
        }

        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            bool flag = _users.ValidateCredentials(context.UserName, context.Password);
            if (flag)
            {
                var identity = GetIdentityPrincipal("1","1", "admin", "Bearer", DateTime.Now, "ars");
                context.Result = new GrantValidationResult(new ClaimsPrincipal(identity));
            }
            else
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidClient, "用户名或密码错误，默认用户名密码为[MyArs:MyArs@1234]");
            }

            return Task.CompletedTask;
        }

        private static ClaimsPrincipal GetIdentityPrincipal(
            string subject,
            string tenant,
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
