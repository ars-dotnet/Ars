using Ars.Commom.Core;
using Ars.Commom.Tool.Certificates;
using Ars.Common.IdentityServer4.options;
using Ars.Common.IdentityServer4.Validation;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.IdentityServer4.Extension
{
    public static class ArsIdentityServerExtension
    {
        public static IArsServiceBuilder AddArsIdentityServer4(
            this IArsServiceBuilder builder,
            Action<ArsIdentityServerOption>? action = null,
            Func<IResourceOwnerPasswordValidator>? func = null)
        {
            ArsIdentityServerOption option = new ArsIdentityServerOption();
            action?.Invoke(option);

            var services = builder.Services.ServiceCollection;
            services.AddIdentityServer()
                .AddArsIdentityResource()
                .AddArsApiResource(option.ArsApiResources)
                .AddArsClients(option.ArsClients)
                .AddArsScopes(option.ArsApiScopes)
                .AddArsSigningCredential(Certificate.Get())
                .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>();

            if (null != func)
                services.AddTransient(_ => func());

            return builder;
        }

        public static IArsServiceBuilder AddArsIs4Authentication(
            this IArsServiceBuilder builder,
            string defaultScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme,
            Action<IdentityServerAuthenticationOptions>? configureOptions = null)  
        {
            var services = builder.Services.ServiceCollection;
            services.AddAuthentication(defaultScheme)
                .AddIdentityServerAuthentication(configureOptions);

            return builder;
        }
    }
}
