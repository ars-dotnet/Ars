using Ars.Common.Core.Configs;
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

        IServiceProvider ServiceScopeProvider { get; }

        IHostBuilder HostBuilder { get; }

        IConfiguration Configuration { get; }

        IWebHostEnvironment Environment { get; }

        ILoggingBuilder Logging { get; }

        IArsConfiguration ArsConfiguration { get; }
    }

    public class ArsWebApplicationBuilder : IArsWebApplicationBuilder 
    {
        private readonly WebApplicationBuilder _webApplicationBuilder;
        private readonly IArsConfiguration _arsConfiguration;
        public ArsWebApplicationBuilder(
            WebApplicationBuilder webApplicationBuilder, 
            IArsConfiguration arsConfiguration)
        {
            _webApplicationBuilder = webApplicationBuilder;
            _arsConfiguration = arsConfiguration;
        }

        public IServiceCollection Services => _webApplicationBuilder.Services;

        public IHostBuilder HostBuilder => _webApplicationBuilder.Host;

        public IConfiguration Configuration => _webApplicationBuilder.Configuration;

        public IServiceProvider ServiceScopeProvider => Services.BuildServiceProvider().CreateScope().ServiceProvider;

        public IWebHostEnvironment Environment => _webApplicationBuilder.Environment;

        public ILoggingBuilder Logging => _webApplicationBuilder.Logging;

        public IArsConfiguration ArsConfiguration => _arsConfiguration;
    }
}
