using Ars.Common.Consul.Option;
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
        /// <summary>
        /// 根据服务名称获取服务地址
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static string GetDomainByServiceName(string serviceName,string consuleAddress)
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
                var queryResult = client.Health.Service(serviceName, string.Empty, true);
                var len = queryResult.Result.Response.Length;
                //平均策略-多个负载中随机获取一个
                var node = queryResult.Result.Response[new Random().Next(len)];
                domain = $"http://{node.Service.Address}:{node.Service.Port}";
            }
            return domain;
        }

        /// <summary>
        /// 获取api域名
        /// </summary>
        /// <returns></returns>
        public static string GetApiDomain(ConsulDiscoverOption option)
        {
            return GetDomainByServiceName(option.ServiceName, option.ConsulAddress);
        }
    }
}
