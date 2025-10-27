using Ars.Common.Core.AspNetCore;
using Duende.IdentityModel;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using Duende.IdentityServer.Validation;
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
                var user = _users.FindByUsername(context.UserName);
                var identity = GetIdentityPrincipal(
                    user.SubjectId, user.Claims.FirstOrDefault(r => r.Type.Equals(ArsClaimTypes.TenantId))?.Value ?? "1",
                    user.Username, user.Claims.FirstOrDefault(r => r.Type.Equals(ArsClaimTypes.Role))?.Value ?? "admin", 
                    "Bearer", DateTime.Now, "ars");
                context.Result = new GrantValidationResult(new ClaimsPrincipal(identity));
            }
            else
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidClient, "用户名或密码错误，默认用户名密码分别为[MyArs:123456]");
            }

            return Task.CompletedTask;
        }

        private ClaimsPrincipal GetIdentityPrincipal(
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
                new Claim(ArsClaimTypes.SetUserId, subject),
                new Claim(ArsClaimTypes.TenantId, tenant),
                new Claim(ArsClaimTypes.Role, userrole),
                new Claim(ArsClaimTypes.UserName, username),
                new Claim("amr", authenticationMethod),
                new Claim("idp", identityProvider),
                //new Claim("auth_time", DateTimeExtensions.ToEpochTime(authTime).ToString(),
                //    "http://www.w3.org/2001/XMLSchema#integer"),
                new Claim("auth_time", new DateTimeOffset(authTime).ToUnixTimeSeconds().ToString(),
                    "http://www.w3.org/2001/XMLSchema#integer")
            };
            var claimsIdentity = new ClaimsIdentity(authenticationMethod);
            claimsIdentity.AddClaims(source.Distinct<Claim>((IEqualityComparer<Claim>)new ClaimComparer()));
            return new ClaimsPrincipal(claimsIdentity);
        }
    }
}
