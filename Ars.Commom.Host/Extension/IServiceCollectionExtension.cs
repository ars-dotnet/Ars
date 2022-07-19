using Ars.Commom.Core;
using Ars.Commom.Tool.Serializer;
using Ars.Common.AutoFac.Dependency;
using Ars.Common.AutoFac.IDependency;
using Ars.Common.AutoFac.Options;
using Ars.Common.AutoFac.RegisterProvider;
using Ars.Common.Core;
using Ars.Common.Core.AspNetCore;
using Ars.Common.Core.Uow;
using Ars.Common.Host;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Commom.Host.Extension
{
    public static class IServiceCollectionExtension
    {
        public static IArsServiceBuilder AddArserviceCore(this IServiceCollection services,
            WebApplicationBuilder builder)
        {
            var arsbuilder = new ArsServiceBuilder(new ArsServiceCollection(services), builder.Host);
            arsbuilder.AddAspNetCore();
            arsbuilder.AddArsAutofac();
            var providerfactory = arsbuilder.Services.Provider.GetRequiredService<IRegisterProviderFactory>();
            builder.Host.UseServiceProviderFactory(new ArsServiceProviderFactory(providerfactory));

            services.AddSingleton<IArsSerializer, ArsSerializer>();
            services.AddSingleton<IArsConfiguration, ArsConfiguration>();
            services.AddSingleton<IUnitOfWorkDefaultConfiguration, UnitOfWorkDefaultConfiguration>();

            return arsbuilder;
        }

        internal static IArsServiceBuilder AddArsAutofac(
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
            //services.AddSingleton<IServiceProviderFactory<ContainerBuilder>>(new ArsServiceProviderFactory(ContainerBuildOptions.None,services.BuildServiceProvider().GetService<IRegisterProviderFactory>(), containerAction));

            return arsServiceBuilder;
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

        internal static IArsServiceBuilder AddAspNetCore(this IArsServiceBuilder arsServiceBuilder)
        {
            var services = arsServiceBuilder.Services.ServiceCollection;
            services.AddHttpContextAccessor();
            services.AddSingleton<IPrincipalAccessor, AspNetCorePrincipalAccessor>();
            services.AddSingleton<IArsAspNetCoreConfiguration,ArsAspNetCoreConfiguration>();
            services.AddScoped<IArsSession, ArsSession>();

            services.AddTransient<UowActionFilter>();
            services.AddControllers(option =>
            {
                option.Filters.AddService<UowActionFilter>();
            })
            .AddNewtonsoftJson(options =>
            {
                //修改属性名称的序列化方式，首字母小写，即驼峰样式
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

                //日期类型默认格式化处理 方式1
                options.SerializerSettings.Converters.Add(new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });

                //忽略循环引用
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                //解决命名不一致问题 
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();

                //空值处理
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });

            return arsServiceBuilder;
        }

    }
}
