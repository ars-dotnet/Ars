using Ars.Commom.Core;
using Ars.Common.AutoFac.Dependency;
using Ars.Common.AutoFac.IDependency;
using Ars.Common.AutoFac.Options;
using Ars.Common.AutoFac.RegisterProvider;
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
        public static IArsServiceBuilder AddArsAutofac(
            this IArsServiceBuilder arsServiceBuilder,
            Action<PropertyAutowiredOption>? autowiredAction = null)
        {
            var services = arsServiceBuilder.Services.ServiceCollection;

            services.AddPropertyAutowired(autowiredAction);
            services.TryAddEnumerable(new[]
            {
                ServiceDescriptor.Singleton<IArsRegisterProvider,ArsInterfaceRegisterProvider>(),
                ServiceDescriptor.Singleton<IArsRegisterProvider,ArsModuleRegisterProvider>(),
                ServiceDescriptor.Singleton<IArsRegisterProvider,ArsPropertyRegisterProvider>(),
            });
            services.AddSingleton<IRegisterProviderFactory, RegisterProviderFactory>();
            var providerfactory = arsServiceBuilder.Services.Provider.GetRequiredService<IRegisterProviderFactory>();
            arsServiceBuilder.HostBuilder.UseServiceProviderFactory(new ArsServiceProviderFactory(providerfactory));
            //services.AddSingleton<IServiceProviderFactory<ContainerBuilder>>(new ArsServiceProviderFactory(ContainerBuildOptions.None,services.BuildServiceProvider().GetService<IRegisterProviderFactory>(), containerAction));

            return arsServiceBuilder;
        }

        private static IServiceCollection AddPropertyAutowired(this IServiceCollection services, Action<PropertyAutowiredOption>? action = null)
        {
            if (null != action) services.Configure(action);
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());

            return services;
        }
    }
}
