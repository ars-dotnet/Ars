using Ars.Common.Consul.Option;
using Ars.Common.Core.Configs;
using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Consul
{
    /// <summary>
    /// Consul帮助类
    /// </summary>
    public class ConsulHelper
    {
        private readonly IConsulDiscoverConfiguration _options;
        public ConsulHelper(IConsulDiscoverConfiguration options)
        {
            _options = options;
        }

        /// <summary>
        /// 根据服务名称获取服务地址
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public async Task<string> GetServiceDomain(string serviceName, string consuleAddress)
        {
            string domain = string.Empty;
            //Consul客户端
            using (ConsulClient client = new ConsulClient(c =>
            {
                c.Address = new Uri(consuleAddress);
                c.Datacenter = "dc1";
            }))
            {
                //根据服务名获取健康的服务
                var queryResult = await client.Health.Service(serviceName, string.Empty, true);
                if (!queryResult?.Response?.Any() ?? false)
                    throw new Exception("get service faild from consul");
                var len = queryResult!.Response.Length;
                //平均策略-多个负载中随机获取一个
                var node = queryResult.Response[new Random().Next(len)];
                domain = $"http://{node.Service.Address}:{node.Service.Port}";
            }
            return domain;
        }

        public Task<string> GetServiceDomain(string serviceName,out bool useHttp1Protocol) 
        {
            var option = _options.ConsulDiscovers?.FirstOrDefault(
                r => serviceName.Equals(r.ServiceName,StringComparison.OrdinalIgnoreCase)) 
                ?? throw new DriveNotFoundException(nameof(serviceName));

            useHttp1Protocol = option.UseHttp1Protocol;
            return GetServiceDomain(serviceName, option.ConsulAddress);
        }
    }
}
