using Ars.Common.Core.Configs;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.IdentityServer4.Extension
{
    public static class IArsConfigurationExtension 
    {
        public static void AddArsIdentityServer(
            this IArsConfiguration arsConfiguration, 
            Func<IServiceProvider, IResourceOwnerPasswordValidator>? func) 
        {
            arsConfiguration.AddArsServiceExtension(new ArsIdentityServerSerivceExtension(func));
        }

        public static void AddArsIdentityClient(
            this IArsConfiguration arsConfiguration,
            Action<IdentityServerAuthenticationOptions>? configureOptions,
            Action<AuthorizationOptions>? configure) 
        {
            arsConfiguration.AddArsServiceExtension(new ArsIdentityClientServiceExtension(configureOptions, configure));
        }
    }
}
