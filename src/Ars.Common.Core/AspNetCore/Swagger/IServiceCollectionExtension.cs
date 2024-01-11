using Ars.Common.Core.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.AspNetCore.Swagger
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddArsSwaggerGen(
            this IServiceCollection services,
            IArsIdentityClientConfiguration? arsIdentity = null)
        {
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfigureOptions>();

            Action<SwaggerGenOptions> setupAction = c =>
            {
                if (null != arsIdentity) 
                {
                    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows
                        {
                            Password = new OpenApiOAuthFlow
                            {
                                AuthorizationUrl = new Uri($"{arsIdentity.Authority}/connect/authorize", UriKind.Absolute),
                                TokenUrl = new Uri($"{arsIdentity.Authority}/connect/token", UriKind.Absolute),
                                //Scopes = new Dictionary<string, string>()
                                //{
                                //    { "grpcapi-scope","授权读写操作" }
                                //}
                            }
                        }
                    });
                }

                string path = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, 
                    $"{AppDomain.CurrentDomain.FriendlyName}.xml");
                if (File.Exists(path))
                {
                    c.IncludeXmlComments(path);
                }

                //枚举显示为字符串
                c.SchemaFilter<EnumSchemaFilter>();
                //根据AuthorizeAttributea分配是否需要授权操作
                c.OperationFilter<SecurityRequirementsOperationFilter>();
                //集成多版本api自定义设置
                c.OperationFilter<SwaggerMultiVersionOperationFilter>();
            };

            return services.AddArsSwaggerGen(setupAction);
        }

        public static IServiceCollection AddArsSwaggerGen(
            this IServiceCollection services,
            Action<SwaggerGenOptions>? setupAction = null)
        {
            return services.AddSwaggerGen(setupAction);
        }
    }
}
