using Ars.Commom.Core;
using Ars.Commom.Tool.Certificates;
using Ars.Common.Core.AspNetCore;
using Ars.Common.Core.Configs;
using Ars.Common.IdentityServer4.options;
using Ars.Common.IdentityServer4.Options;
using Ars.Common.IdentityServer4.Validation;
using Duende.IdentityModel;
using Duende.IdentityServer.Test;
using Duende.IdentityServer.Validation;
using IdentityModel.AspNetCore.OAuth2Introspection;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
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
                            //new Claim("auth_time", DateTimeExtensions.ToEpochTime(DateTime.Now).ToString(),
                            //        "http://www.w3.org/2001/XMLSchema#integer"),
                            new Claim("auth_time", new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString(),
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
        [Obsolete]
        public static IArsWebApplicationBuilder AddArsIdentityClient(
            this IArsWebApplicationBuilder builder,
            string defaultScheme = JwtBearerDefaults.AuthenticationScheme,
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
                    if(null != option.JwtValidationClockSkew)
                        t.JwtValidationClockSkew = TimeSpan.FromSeconds(option.JwtValidationClockSkew.Value);

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

                    t.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = ctx =>
                        {
                            var logger = ctx.HttpContext.RequestServices.GetService<Microsoft.Extensions.Logging.ILoggerFactory>()?
                                .CreateLogger("JwtBearer");
                            logger?.LogError(ctx.Exception, "Jwt authentication failed");
                            return Task.CompletedTask;
                        },

                    };

                    //t.TokenValidationParameters = new TokenValidationParameters
                    //{
                    //    ValidateAudience = false,
                    //    ValidateIssuer = false,
                    //    ValidateIssuerSigningKey = false,
                    //    SignatureValidator = delegate (string token, TokenValidationParameters parameters)
                    //    {
                    //        var jwt = new JwtSecurityToken(token);

                    //        return jwt;
                    //    },
                    //};
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

        /// <summary>
        /// 注册IdentityServer4认证的资源服务端
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="defaultScheme"></param>
        /// <param name="configureJwt"></param>
        /// <param name="configureIntrospection"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static IArsWebApplicationBuilder AddArsIdentityClient(
            this IArsWebApplicationBuilder builder,
            string defaultScheme = JwtBearerDefaults.AuthenticationScheme,
            Action<JwtBearerOptions>? configureJwt = null,
            Action<OAuth2IntrospectionOptions>? configureIntrospection = null,
            Action<AuthorizationOptions>? configure = null)
        {
            var option = builder.Configuration
                .GetSection(nameof(ArsIdentityClientConfiguration))
                .Get<ArsIdentityClientConfiguration>()
                ?? throw new Exception("appsetting => ArsIdentityClientConfiguration not be null!");

            option.CertificatePath ??= builder.ArsConfiguration.ArsBasicConfiguration?.CertificatePath;
            option.CertificatePassWord ??= builder.ArsConfiguration.ArsBasicConfiguration?.CertificatePassWord;

            builder.ArsConfiguration.ArsIdentityClientConfiguration ??= option;
            builder.ArsConfiguration.AddArsAppExtension(new ArsIdentityClientAppExtension());

            var services = builder.Services;
            services.AddSingleton<IArsIdentityClientConfiguration>(_ => option);

            // Default authorization policy if none provided
            if (configure == null)
            {
                configure = options =>
                {
                    options.AddPolicy("default", policy => policy.AddRequirements(new DefaultAuthorizationRequirement()));
                };
            }

            // Decide whether to use introspection (reference tokens) or JWT.
            // You can add a flag in ArsIdentityClientConfiguration like UseIntrospection or detect presence of Introspection client credentials.
            //var useIntrospection = option.UseIntrospection
            //                      || (!string.IsNullOrEmpty(option.IntrospectionClientId) && !string.IsNullOrEmpty(option.IntrospectionClientSecret));

            // JWT bearer
            services.AddAuthentication(defaultScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = option.Authority;
                    options.Audience = option.ApiName;
                    options.RequireHttpsMetadata = option.RequireHttpsMetadata;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateIssuerSigningKey = false,
                        SignatureValidator = delegate (string token, TokenValidationParameters parameters)
                        {
                            var jwt = new JsonWebToken(token);

                            return jwt;
                        },
                    };

                    if (option.JwtValidationClockSkew.HasValue)
                        options.TokenValidationParameters.ClockSkew = TimeSpan.FromSeconds(option.JwtValidationClockSkew.Value);

                    if (options.RequireHttpsMetadata)
                    {
                       var httpClientHandler = new HttpClientHandler
                        {
                            ClientCertificateOptions = ClientCertificateOption.Manual,
                            SslProtocols = SslProtocols.Tls12,
                            // NOTE: DangerousAcceptAnyServerCertificateValidator kept for parity with original;
                            // in production please validate certs correctly.
                            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                        };
                        httpClientHandler.ClientCertificates.Add(
                            Certificate.Get(option.CertificatePath!, option.CertificatePassWord!));

                        options.BackchannelHttpHandler = httpClientHandler;
                    }

                    // Allow token via query string for SignalR
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = ctx =>
                        {
                            // Try header first, then query string "access_token"
                            var authHeader = ctx.Request.Headers["Authorization"].FirstOrDefault();
                            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                            {
                                ctx.Token = authHeader.Substring("Bearer ".Length).Trim();
                                return Task.CompletedTask;
                            }

                            var token = ctx.Request.Query["access_token"].FirstOrDefault();
                            if (!string.IsNullOrEmpty(token))
                            {
                                ctx.Token = token;
                            }
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = ctx =>
                        {
                            var logger = ctx.HttpContext.RequestServices.GetService<Microsoft.Extensions.Logging.ILoggerFactory>()?
                                .CreateLogger("JwtBearer");
                            logger?.LogError(ctx.Exception, "Jwt authentication failed");
                            return Task.CompletedTask;
                        },

                    };

                    configureJwt?.Invoke(options);
                });

            // Introspection (reference tokens)
            //services.AddAuthentication("introspection")
            //    .AddOAuth2Introspection("introspection", options =>
            //    {
            //        options.Authority = option.Authority;
            //        //options.ClientId = option.IntrospectionClientId;
            //        //options.ClientSecret = option.IntrospectionClientSecret;
            //        options.DiscoveryPolicy.RequireHttps = option.RequireHttpsMetadata;

            //        if (backChannelHandler != null)
            //            options.BackChannelHandler = backChannelHandler;

            //        if (option.JwtValidationClockSkew.HasValue)
            //            options.ClockSkew = TimeSpan.FromSeconds(option.JwtValidationClockSkew.Value);

            //        // TokenRetriever similar to original: header then query
            //        options.TokenRetriever = request =>
            //        {
            //            var auth = request.Headers["Authorization"].FirstOrDefault();
            //            if (!string.IsNullOrEmpty(auth) && auth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            //                return auth.Substring("Bearer ".Length).Trim();

            //            return request.Query["access_token"].FirstOrDefault();
            //        };

            //        // small caching helpful in high throughput
            //        options.EnableCaching = true;
            //        options.CacheDuration = TimeSpan.FromMinutes(5);

            //        configureIntrospection?.Invoke(options);
            //    });

            services.AddAuthorization(configure);

            return builder;
        }
    }
}
