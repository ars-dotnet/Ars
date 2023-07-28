using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.AspNetCore.HostService
{
    internal class ArsManualExecutingManager : IArsManualExecutingManager
    {
        private readonly IEnumerable<IArsManualExecutingService> _executingServices;
        public ArsManualExecutingManager(IEnumerable<IArsManualExecutingService> executingServices)
        {
            _executingServices = executingServices; 
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            foreach (var service in _executingServices) 
            {
                await service.StartAsync(cancellationToken);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            foreach (var service in _executingServices)
            {
                await service.StopAsync(cancellationToken);
            }
        }
    }
}
