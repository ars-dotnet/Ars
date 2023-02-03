using Ars.Commom.Core;
using Ars.Common.Core.AspNetCore;
using Ars.Common.Core.Configs;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Ars.Common.Core.Extension
{
    public static class IServiceCollectionExtension
    {
        public static IArsServiceBuilder AddArsAspNetCore(this IArsServiceBuilder arsServiceBuilder)
        {
            var services = arsServiceBuilder.Services.ServiceCollection;
            services.AddHttpContextAccessor();
            services.AddSingleton<IPrincipalAccessor, AspNetCorePrincipalAccessor>();
            services.AddSingleton<IArsAspNetCoreConfiguration, ArsAspNetCoreConfiguration>();
            services.AddSingleton<IUnitOfWorkDefaultConfiguration, UnitOfWorkDefaultConfiguration>();
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
