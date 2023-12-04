using Ars.Commom.Core;
using Ars.Common.AutoFac.Dependency;
using Ars.Common.AutoFac.IDependency;
using Ars.Common.AutoFac.Options;
using Ars.Common.AutoFac.RegisterProvider;
using Ars.Common.Core;
using Ars.Common.Core.Configs;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.AutoFac.Extension
{
    public static class IServiceCollectionExtension
    {
        public static IArsWebApplicationBuilder AddAutofac(
            this IArsWebApplicationBuilder arsServiceBuilder,
            Action<PropertyAutowiredOption>? _autowiredAction) 
        {
            var services = arsServiceBuilder.Services;
            AddPropertyAutowired(services, _autowiredAction);
            services.TryAddEnumerable(new[]
            {
                ServiceDescriptor.Singleton<IArsRegisterProvider,ArsInterfaceRegisterProvider>(),
                ServiceDescriptor.Singleton<IArsRegisterProvider,ArsModuleRegisterProvider>(),
                ServiceDescriptor.Singleton<IArsRegisterProvider,ArsPropertyRegisterProvider>(),
            });
            services.AddSingleton<IRegisterProviderFactory, RegisterProviderFactory>();

            arsServiceBuilder.HostBuilder.UseServiceProviderFactory(
                new ArsServiceProviderFactory(arsServiceBuilder.ServiceScopeProvider));

            return arsServiceBuilder;
        }

        private static IServiceCollection AddPropertyAutowired(
            IServiceCollection services, 
            Action<PropertyAutowiredOption>? _autowiredAction)
        {
            if (null != _autowiredAction) services.Configure(_autowiredAction);
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());

            return services;
        }
    }
}
