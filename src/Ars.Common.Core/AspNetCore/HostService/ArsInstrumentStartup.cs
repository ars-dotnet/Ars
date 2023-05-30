using Ars.Common.Core.IDependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.AspNetCore.HostService
{
    /// <summary>
    /// 执行程序启动时的任务
    /// </summary>
    internal class ArsInstrumentStartup : IArsInstrumentStartup,IScopedDependency
    {
        private readonly IEnumerable<IArsHostStartupExecutingService> _executingServices;
        public ArsInstrumentStartup(IEnumerable<IArsHostStartupExecutingService> executingServices)
        {
            _executingServices = executingServices;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var service in _executingServices) 
            {
                await service.StartAsync(cancellationToken);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var service in _executingServices) 
            {
                await service.StopAsync(cancellationToken);
            }
        }
    }
}
