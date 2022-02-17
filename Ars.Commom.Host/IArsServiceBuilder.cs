using Microsoft.Extensions.Hosting;

namespace Ars.Commom.Host
{
    public interface IArsServiceBuilder
    {
        IArsServiceCollection Services { get; }

        IHostBuilder HostBuilder { get; }
    }

    public class ArsServiceBuilder : IArsServiceBuilder 
    {
        private readonly IArsServiceCollection _services;
        private readonly IHostBuilder _hostBuilder;
        public ArsServiceBuilder(IArsServiceCollection services, IHostBuilder hostBuilder)
        {
            _services = services;
            _hostBuilder = hostBuilder;
        }

        public IArsServiceCollection Services => _services;

        public IHostBuilder HostBuilder => _hostBuilder;
    }
}
