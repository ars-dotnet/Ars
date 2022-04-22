using Ars.Common.Consul.Option;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Consul.IApplicationBuilderExtension
{
    public static class IApplicationBuilderExtension
    {
        public static IApplicationBuilder UseConsul(this IApplicationBuilder app, IConfiguration configuration)
        {
            // 获取主机生命周期管理接口
            var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
            var option = app.ApplicationServices.GetRequiredService<IOptions<ConsulRegisteOption>>().Value;
            ConsulClient client = new ConsulClient(c =>
            {
                c.Address = new Uri(option.ConsulAddress);
                c.Datacenter = "dc1";
            });
            string ip = configuration["ip"]; //优先接收变量的值
            int.TryParse(configuration["port"],out int port); //优先接收变量的值
            string currentIp = option.ServiceIp;
            int currentPort = option.ServicePort;

            ip = string.IsNullOrEmpty(ip) ? currentIp : ip; //当前程序的IP
            port = 0 == port ? currentPort : port; //当前程序的端口
            string serviceId = $"service:{ip}:{port}";//服务ID，一个服务是唯一的
            //服务注册
            client.Agent.ServiceRegister(new AgentServiceRegistration()
            {
                ID = serviceId, //唯一的
                Name = option.ServiceName, //组名称-Group
                Address = ip, //ip地址
                Port = port, //端口
                Tags = new string[] { option.ServiceName },
                Check = new AgentServiceCheck()
                {
                    Interval = TimeSpan.FromSeconds(10),//多久检查一次心跳
                    GRPC = $"{ip}:{port}", //gRPC注册特有
                    GRPCUseTLS = false,//支持http
                    Timeout = TimeSpan.FromSeconds(5),//超时时间
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5) //服务停止多久后注销服务
                }
            }).Wait();
            //应用程序终止时,注销服务
            lifetime.ApplicationStopping.Register(() =>
            {
                client.Agent.ServiceDeregister(serviceId).Wait();
            });
            return app;
        }
    }
}
