using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ars.Commom.Core
{
    public interface IArsServiceBuilder
    {
        IServiceCollection Services { get; }

        IHostBuilder HostBuilder { get; }

        IConfiguration Configuration { get; }

        IServiceProvider ServiceProvider { get; }
    }

    public class ArsServiceBuilder : IArsServiceBuilder 
    {
        private readonly IServiceCollection _services;

        private readonly IHostBuilder _hostBuilder;

        private readonly IConfiguration _configuration;
        public ArsServiceBuilder(
            IServiceCollection services, 
            IHostBuilder hostBuilder,
            IConfiguration configuration)
        {
            _services = services;
            _hostBuilder = hostBuilder;
            _configuration = configuration;
        }

        public IServiceCollection Services => _services;

        public IHostBuilder HostBuilder => _hostBuilder;

        public IConfiguration Configuration => _configuration;

        public IServiceProvider ServiceProvider => _services.BuildServiceProvider();
    }
}
