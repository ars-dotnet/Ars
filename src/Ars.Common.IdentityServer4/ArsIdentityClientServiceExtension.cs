using Ars.Commom.Core;
using Ars.Common.Core;
using Ars.Common.Core.Configs;
using Ars.Common.IdentityServer4.Extension;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.IdentityServer4
{
    internal class ArsIdentityClientServiceExtension : IArsServiceExtension
    {
        private readonly Action<IdentityServerAuthenticationOptions>? _configureOptions;
        private readonly Action<AuthorizationOptions>? _configure;
        public ArsIdentityClientServiceExtension(
            Action<IdentityServerAuthenticationOptions>? configureOptions,
            Action<AuthorizationOptions>? configure)
        {
            _configureOptions = configureOptions;
            _configure = configure;
        }

        public void AddService(IArsWebApplicationBuilder services)
        {
            services.AddArsIdentityClient(
                JwtBearerDefaults.AuthenticationScheme,
                null,null, _configure);
        }
    }
}
