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
        public static IArsConfiguration AddArsIdentityServer(
            this IArsConfiguration arsConfiguration,
            Func<IServiceProvider, IResourceOwnerPasswordValidator>? func = null) 
        {
            return arsConfiguration.AddArsServiceExtension(
                new ArsIdentityServerSerivceExtension(func));
        }

        public static IArsConfiguration AddArsIdentityClient(
            this IArsConfiguration arsConfiguration,
            Action<IdentityServerAuthenticationOptions>? configureOptions = null,
            Action<AuthorizationOptions>? configure = null) 
        {
            return arsConfiguration.AddArsServiceExtension(
                new ArsIdentityClientServiceExtension(configureOptions, configure));
        }
    }
}
