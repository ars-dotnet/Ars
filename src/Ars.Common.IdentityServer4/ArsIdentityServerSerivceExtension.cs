﻿using Ars.Commom.Core;
using Ars.Common.Core;
using Ars.Common.Core.Configs;
using Ars.Common.IdentityServer4.Extension;
using Duende.IdentityServer.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.IdentityServer4
{
    internal class ArsIdentityServerSerivceExtension : IArsServiceExtension
    {
        private readonly Func<IServiceProvider, IResourceOwnerPasswordValidator>? _func;
        public ArsIdentityServerSerivceExtension(Func<IServiceProvider, IResourceOwnerPasswordValidator>? func)
        {
            _func = func;
        }

        public void AddService(IArsWebApplicationBuilder services)
        {
            services.AddArsIdentityServer(_func);
        }
    }
}
