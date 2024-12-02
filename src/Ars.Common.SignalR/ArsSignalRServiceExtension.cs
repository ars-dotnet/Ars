using Ars.Commom.Core;
using Ars.Common.Core;
using Ars.Common.Core.Configs;
using Ars.Common.SignalR.Extensions;
using Microsoft.AspNetCore.SignalR;
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
        private readonly Action<HubOptions>? _action1;

        public ArsSignalRServiceExtension(Action<ArsSignalRConfiguration>? action, Action<HubOptions>? action1)
        {
            _action = action;
            _action1 = action1;
        }

        public void AddService(IArsWebApplicationBuilder services)
        {
            services.AddArsSignalR(_action, _action1);
        }
    }
}
