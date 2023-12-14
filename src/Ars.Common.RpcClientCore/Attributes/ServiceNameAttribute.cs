using Ars.Common.Consul;
using Ars.Common.Core.Configs;
using Ars.Common.Tool;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClientCore;
using WebApiClientCore.Attributes;

namespace Ars.Common.RpcClientCore.Attributes
{
    /// <summary>
    /// 通过serviceName获取host
    /// </summary>
    public class ServiceNameAttribute : ApiActionAttribute
    {
        private readonly string _serviceName;
        public ServiceNameAttribute(string serviceName)
        {
            _serviceName = serviceName;

            OrderIndex = 101;
        }

        public override async Task OnRequestAsync(ApiRequestContext context)
        {
            string host = await GetHostFromConsulAsync(context.HttpContext.ServiceProvider);

            HttpApiRequestMessage requestMessage = context.HttpContext.RequestMessage;
            //和原有的Uri组合并覆盖原有Uri
            //并非一定要这样实现，只要覆盖了RequestUri,即完成了替换
            context.HttpContext.RequestMessage.RequestUri = requestMessage.MakeRequestUri(new Uri(host));

            context.Properties.Set("serviceName",_serviceName);
        }

        /// <summary>
        /// 从consul中获取host
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        private Task<string> GetHostFromConsulAsync(IServiceProvider provider)
        {
            IConsulDiscoverConfiguration config = provider.GetRequiredService<IConsulDiscoverConfiguration>();
            
            var option = config.ConsulDiscovers.
                   FirstOrDefault(r => r.ServiceName.Equals(_serviceName, StringComparison.OrdinalIgnoreCase))
                ?? throw new ArsException($"consul service:{_serviceName} not find in rpcClientCore framework");

            ConsulHelper consulHelper = provider.GetRequiredService<ConsulHelper>();

            return consulHelper.GetServiceDomain(option.ServiceName, option.ConsulAddress);
        }
    }
}
