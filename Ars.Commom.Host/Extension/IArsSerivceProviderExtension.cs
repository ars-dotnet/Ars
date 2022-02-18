using Ars.Commom.Core;
using Ars.Common.AutoFac.Dependency;
using Ars.Common.AutoFac.IDependency;
using Ars.Common.AutoFac.Options;
using Ars.Common.AutoFac.RegisterProvider;
using Ars.Common.Host;
using Autofac;
using Autofac.Builder;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Commom.Host.Extension
{
    public static class IArsSerivceProviderExtension
    {
        internal static IArsServiceBuilder AddArsAutofac(
            this IArsServiceBuilder arsServiceProvider, 
            Action<PropertyAutowiredOption>? autowiredAction = null) 
        {
            var services = arsServiceProvider.Services.ServiceCollection;

            services.AddPropertyAutowired(autowiredAction);
            services.TryAddEnumerable(new[]
            {
                ServiceDescriptor.Singleton<IArsRegisterProvider,ArsInterfaceRegisterProvider>(),
                ServiceDescriptor.Singleton<IArsRegisterProvider,ArsModuleRegisterProvider>(),
                ServiceDescriptor.Singleton<IArsRegisterProvider,ArsPropertyRegisterProvider>(),
            });
            services.AddSingleton<IRegisterProviderFactory, RegisterProviderFactory>();
            //services.AddSingleton<IServiceProviderFactory<ContainerBuilder>>(new ArsServiceProviderFactory(ContainerBuildOptions.None,services.BuildServiceProvider().GetService<IRegisterProviderFactory>(), containerAction));

            return arsServiceProvider;
        }

        private static IServiceCollection AddPropertyAutowired(this IServiceCollection services, Action<PropertyAutowiredOption>? action = null)
        {
            PropertyAutowiredOption option = new PropertyAutowiredOption();
            action?.Invoke(option);

            services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());
            if (null != action)
                services.Configure(action);

            return services;
        }
    }
}
