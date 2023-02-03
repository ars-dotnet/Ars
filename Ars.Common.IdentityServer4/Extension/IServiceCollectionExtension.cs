using Ars.Commom.Core;
using Ars.Commom.Tool.Certificates;
using Ars.Common.Core.Configs;
using Ars.Common.IdentityServer4.options;
using Ars.Common.IdentityServer4.Options;
using Ars.Common.IdentityServer4.Validation;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.IdentityServer4.Extension
{
    public static class IServiceCollectionExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="action"></param>
        /// <param name="func">自定义实现类</param>
        /// <returns></returns>
        public static IArsServiceBuilder AddArsIdentityServer(
            this IArsServiceBuilder builder,
            Func<IServiceProvider,IResourceOwnerPasswordValidator>? func = null)
        {
            var services = builder.Services.ServiceCollection;
            var option = builder.Services.Provider
                .GetRequiredService<IConfiguration>()
                .GetSection(nameof(ArsIdentityServerOption))
                .Get<ArsIdentityServerOption>() ?? new ArsIdentityServerOption();

            services.AddSingleton<IArsIdentityServerConfiguration>(option);
            builder.Services.Provider.GetRequiredService<IArsConfiguration>().ArsIdentityServerConfiguration ??= option;

            services.AddIdentityServer()
                .AddArsIdentityResource()
                .AddArsApiResource(option.ArsApiResources)
                .AddArsClients(option.ArsClients)
                .AddArsScopes(option.ArsApiScopes)
                .AddArsSigningCredential(
                    string.IsNullOrEmpty(option.CertPath) 
                        ? Certificate.Get() 
                        : Certificate.Get(option.CertPath,option.Password))
                .AddResourceOwnerValidator<DefaultResourceOwnerPasswordValidator>();

            if (null != func) 
            {
                services.Replace(new ServiceDescriptor(
                    typeof(IResourceOwnerPasswordValidator),
                    provider => func(provider),
                    ServiceLifetime.Transient));
            }

            return builder;
        }

        public static IArsServiceBuilder AddArsIdentityClient(
            this IArsServiceBuilder builder,
            string defaultScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme,
            Action<IdentityServerAuthenticationOptions>? configureOptions = null)  
        {
            var services = builder.Services.ServiceCollection;
            var option = builder.Services.Provider
                .GetRequiredService<IConfiguration>()
                .GetSection(nameof(ArsIdentityClientOption))
                .Get<ArsIdentityClientOption>() 
                ?? 
                throw new Exception("appsetting => ArsIdentityClientOption not be null!");
            builder.Services.Provider
                .GetRequiredService<IArsConfiguration>()
                .ArsIdentityClientConfiguration ??= option;
            services.AddSingleton<IArsIdentityClientConfiguration>(option);

            if (null == configureOptions) 
            {
                configureOptions = t =>
                {
                    t.Authority = option.Authority;
                    t.ApiName = option.ApiName;
                    t.RequireHttpsMetadata = false;
                };
            }
            services.AddAuthentication(defaultScheme)
                .AddIdentityServerAuthentication(configureOptions);

            return builder;
        }
    }
}
