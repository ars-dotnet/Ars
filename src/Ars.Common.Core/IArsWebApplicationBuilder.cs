using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ars.Commom.Core
{
    public interface IArsWebApplicationBuilder
    {
        IServiceCollection Services { get; }

        IServiceProvider ServiceProvider { get; }

        IHostBuilder HostBuilder { get; }

        IConfiguration Configuration { get; }

        IWebHostEnvironment Environment { get; }

        ILoggingBuilder Logging { get; }
    }

    public class ArsWebApplicationBuilder : IArsWebApplicationBuilder 
    {
        private readonly WebApplicationBuilder _webApplicationBuilder;
        public ArsWebApplicationBuilder(WebApplicationBuilder webApplicationBuilder)
        {
            _webApplicationBuilder = webApplicationBuilder;
        }

        public IServiceCollection Services => _webApplicationBuilder.Services;

        public IHostBuilder HostBuilder => _webApplicationBuilder.Host;

        public IConfiguration Configuration => _webApplicationBuilder.Configuration;

        public IServiceProvider ServiceProvider => Services.BuildServiceProvider();

        public IWebHostEnvironment Environment => _webApplicationBuilder.Environment;

        public ILoggingBuilder Logging => _webApplicationBuilder.Logging;
    }
}
