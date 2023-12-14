using Ars.Commom.Core;
using Ars.Commom.Tool.Certificates;
using Ars.Common.Core.AspNetCore;
using Ars.Common.Core.Configs;
using Ars.Common.IdentityServer4.options;
using Ars.Common.IdentityServer4.Options;
using Ars.Common.IdentityServer4.Validation;
using IdentityModel;
using IdentityModel.AspNetCore.OAuth2Introspection;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Test;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.IdentityServer4.Extension
{
    public static class IServiceCollectionExtension
    {
        /// <summary>
        /// 注册IdentityServer4服务端
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="action"></param>
        /// <param name="func">自定义实现类</param>
        /// <returns></returns>
        public static IArsWebApplicationBuilder AddArsIdentityServer(
            this IArsWebApplicationBuilder builder,
            Func<IServiceProvider,IResourceOwnerPasswordValidator>? func = null)
        {
            var option = builder.Configuration
                .GetSection(nameof(ArsIdentityServerConfiguration))
                .Get<ArsIdentityServerConfiguration>() 
                ?? 
                throw new Exception("appsettings => ArsIdentityServerConfiguration not be null!");

            option.CertificatePath ??= builder.ArsConfiguration.ArsBasicConfiguration?.CertificatePath;
            option.CertificatePassWord ??= builder.ArsConfiguration.ArsBasicConfiguration?.CertificatePassWord;

            builder.ArsConfiguration.ArsIdentityServerConfiguration ??= option;

            builder.ArsConfiguration.AddArsAppExtension(new ArsIdentityServerAppExtension());

            var services = builder.Services;

            services.AddSingleton<IArsIdentityServerConfiguration>(_ => option);

            using var loggerfac = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerfac.CreateLogger(nameof(AddArsIdentityServer));

            var id4builder = services.AddIdentityServer()
                .AddArsIdentityResource()
                .AddArsApiResource(option.ArsApiResources)
                .AddArsClients(option.ArsClients)
                .AddArsScopes(option.ArsApiScopes)
                .AddArsSigningCredential(
                    Certificate.Get(
                        option.CertificatePath!, option.CertificatePassWord!, logger));

            if (option.UseTestUsers) 
            {
                id4builder.AddTestUsers(new List<TestUser>
                {
                    new TestUser
                    {
                        Username = "MyArs",
                        Claims = new Claim[] {
                            new Claim(ArsClaimTypes.TenantId, "1"),
                            new Claim(ArsClaimTypes.Role, "admin"),
                            new Claim(ArsClaimTypes.UserName, "MyArs"),
                            new Claim("idp", "ars"),
                            new Claim("auth_time", DateTimeExtensions.ToEpochTime(DateTime.Now).ToString(),
                                    "http://www.w3.org/2001/XMLSchema#integer")
                        },
                        Password = "123456",
                        IsActive = true,
                        SubjectId = "1",
                    }
                });
            };

            if (null == func)
            {
                id4builder.AddResourceOwnerValidator<DefaultResourceOwnerPasswordValidator>();
            }
            else 
            {
                services.AddTransient(func);
            }

            return builder;
        }

        /// <summary>
        /// 注册IdentityServer4认证的资源服务端
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="defaultScheme"></param>
        /// <param name="configureOptions"></param>
        /// <param name="configure"></param>
        /// <param name="arsConfiguration"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public static IArsWebApplicationBuilder AddArsIdentityClient(
            this IArsWebApplicationBuilder builder,
            string defaultScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme,
            Action<IdentityServerAuthenticationOptions>? configureOptions = null,
            Action<AuthorizationOptions>? configure = null)  
        {
            var option = builder.Configuration
                .GetSection(nameof(ArsIdentityClientConfiguration))
                .Get<ArsIdentityClientConfiguration>() 
                ?? 
                throw new Exception("appsetting => ArsIdentityClientConfiguration not be null!");   

            option.CertificatePath ??= builder.ArsConfiguration.ArsBasicConfiguration?.CertificatePath;
            option.CertificatePassWord ??= builder.ArsConfiguration.ArsBasicConfiguration?.CertificatePassWord;

            builder.ArsConfiguration.ArsIdentityClientConfiguration ??= option;

            builder.ArsConfiguration.AddArsAppExtension(new ArsIdentityClientAppExtension());

            var services = builder.Services;

            services.AddSingleton<IArsIdentityClientConfiguration>(_ => option);

            if (null == configureOptions)
            {
                configureOptions = t =>
                {
                    t.Authority = option.Authority;
                    t.ApiName = option.ApiName;
                    t.RequireHttpsMetadata = option.RequireHttpsMetadata;

                    if (t.RequireHttpsMetadata)
                    {
                        var httpClientHandler = new HttpClientHandler
                        {
                            ClientCertificateOptions = ClientCertificateOption.Manual,
                            SslProtocols = SslProtocols.Tls12,
                            ServerCertificateCustomValidationCallback =
                                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                        };
                        httpClientHandler.ClientCertificates.Add(
                            Certificate.Get(option.CertificatePath!, option.CertificatePassWord!));

                        t.JwtBackChannelHandler = httpClientHandler;
                    }

                    //for signalr token
                    t.TokenRetriever = new Func<HttpRequest, string>(req =>
                    {
                        var fromHeader = TokenRetrieval.FromAuthorizationHeader("Bearer");
                        var fromQuery = TokenRetrieval.FromQueryString("access_token");
                        return fromHeader(req) ?? fromQuery(req);
                    });
                };
            }

            if (null == configure) 
            {
                configure = t => 
                {
                    t.AddPolicy("default",policy => policy.AddRequirements(new DefaultAuthorizationRequirement()));
                };
            }

            services
                .AddAuthentication(defaultScheme)
                .AddIdentityServerAuthentication(configureOptions);

            services.AddAuthorization(configure);

            return builder;
        }
    }
}
