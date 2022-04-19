using Ars.Commom.Host;
using Ars.Commom.Tool.Serializer;
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

namespace Ars.Common.Host
{
    public class ArsServiceProviderFactory : IServiceProviderFactory<ContainerBuilder>
    {
        private readonly Action<ContainerBuilder> _configurationAction;
        private readonly ContainerBuildOptions _containerBuildOptions;
        private readonly IRegisterProviderFactory _registerProviderFactory;

        public ArsServiceProviderFactory(IRegisterProviderFactory factory, ContainerBuildOptions containerBuildOptions = ContainerBuildOptions.None, Action<ContainerBuilder> configurationAction = null)
        {
            _registerProviderFactory = factory;
            _containerBuildOptions = containerBuildOptions;
            _configurationAction = configurationAction ?? (builder => { });
        }

        public ContainerBuilder CreateBuilder(IServiceCollection services)
        {
            var builder = new ContainerBuilder();
            builder.Populate(services);
            _configurationAction(builder);
            return builder;
        }

        public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
        {
            if (containerBuilder == null)
            {
                throw new ArgumentNullException("containerBuilder");
            }

            _registerProviderFactory.Register(containerBuilder, AppDomain.CurrentDomain.GetAssemblies());

            IContainer container = containerBuilder.Build(_containerBuildOptions);

            _registerProviderFactory.RegisterAutowaired(container);

            return new AutofacServiceProvider(container);
        }
    }
}
