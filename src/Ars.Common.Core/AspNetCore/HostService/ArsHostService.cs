using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.AspNetCore.HostService
{
    internal class ArsHostService : IHostedService
    {
        private readonly IArsInstrumentStartup _startup;
        public ArsHostService(IArsInstrumentStartup startup)
        {
            _startup = startup; 
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _startup.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _startup.StopAsync(cancellationToken);
        }
    }
}
