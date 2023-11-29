using Ars.Commom.Tool.Extension;
using Ars.Common.Core.IDependency;
using Ars.Common.Tool;
using Ars.Common.Tool.Extension;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.IdentityServer4.Validation
{
    public class DefaultAuthorizationRequirement : IAuthorizationRequirement
    {

    }

    public class DefaultAuthorizationHandler : AuthorizationHandler<DefaultAuthorizationRequirement>,ISingletonDependency
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, DefaultAuthorizationRequirement requirement)
        {
            var user = context.User;
            if (!user.Logined())
            {
                if (user.GetValue("client_id").IsNullOrEmpty()) 
                {
                    Valid.ThrowException(401, "身份未认证，请重新登录");
                }

                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            if ("admin".Equals(user.GetValue(ClaimTypes.Role)))
            {
                context.Succeed(requirement);
            }
            else
            {
                Valid.ThrowException(403, "没有操作权限");
            }

            return Task.CompletedTask;
        }
    }
}
