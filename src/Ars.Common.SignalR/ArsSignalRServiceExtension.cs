﻿using Ars.Commom.Core;
using Ars.Common.Core;
using Ars.Common.Core.Configs;
using Ars.Common.SignalR.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.SignalR
{
    internal class ArsSignalRServiceExtension : IArsServiceExtension
    {
        private readonly Action<ArsSignalRConfiguration>? _action;
        public ArsSignalRServiceExtension(Action<ArsSignalRConfiguration>? action)
        {
            _action = action;
        }

        public void AddService(IArsWebApplicationBuilder services)
        {
            services.AddArsSignalR(_action);
        }
    }
}
