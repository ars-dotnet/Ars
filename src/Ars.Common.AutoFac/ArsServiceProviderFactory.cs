using Ars.Common.AutoFac.RegisterProvider;
using Autofac;
using Autofac.Builder;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.AutoFac
{
    public class ArsServiceProviderFactory : IServiceProviderFactory<ContainerBuilder>
    {
        private readonly IServiceProvider _serviceScopeProvider;
        private readonly ContainerBuildOptions _containerBuildOptions;
        private readonly Action<ContainerBuilder>? _configurationAction;

        public ArsServiceProviderFactory(
            IServiceProvider serviceScopeProvider,
            ContainerBuildOptions containerBuildOptions = ContainerBuildOptions.None,
            Action<ContainerBuilder>? configurationAction = null)
        {
            _serviceScopeProvider = serviceScopeProvider;
            _containerBuildOptions = containerBuildOptions;
            _configurationAction = configurationAction;
        }

        public ContainerBuilder CreateBuilder(IServiceCollection services)
        {
            var builder = new ContainerBuilder();

            builder.Populate(services);

            _configurationAction?.Invoke(builder);

            return builder;
        }

        public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
        {
            if (containerBuilder == null)
            {
                throw new ArgumentNullException("containerBuilder");
            }

            var registerProviderFactory = _serviceScopeProvider.GetRequiredService<IRegisterProviderFactory>();

            registerProviderFactory.Register(containerBuilder, AppDomain.CurrentDomain.GetAssemblies());

            IContainer container = containerBuilder.Build(_containerBuildOptions);

            registerProviderFactory.RegisterAutowaired(container);

            return new AutofacServiceProvider(container);
        }
    }
}
