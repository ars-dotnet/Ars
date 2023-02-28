using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.Swagger
{
    public class SecurityRequirementsOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            //获取是否添加登录特性
            var authAttributes =
                  !context.MethodInfo.DeclaringType!.GetCustomAttributes(true)
                 .Union(context.MethodInfo.GetCustomAttributes(true))
                 .OfType<AllowAnonymousAttribute>().Any()
                 &&
                  context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                 .Union(context.MethodInfo.GetCustomAttributes(true))
                 .OfType<AuthorizeAttribute>().Any();

            if (authAttributes)
            {
                operation.Responses.Add("401", new OpenApiResponse { Description = "暂无访问权限" });
                operation.Responses.Add("403", new OpenApiResponse { Description = "禁止访问" });
                var oAuthScheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                };
                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        [ oAuthScheme ] = new List<string>{ "Authorize" }
                    }
                };
            }
        }
    }
}
