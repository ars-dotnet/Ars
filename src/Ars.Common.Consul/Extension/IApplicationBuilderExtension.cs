﻿using Ars.Commom.Tool.Certificates;
using Ars.Common.Consul.HttpClientHelper;
using Ars.Common.Consul.Option;
using Ars.Common.Core;
using Ars.Common.Core.Configs;
using Ars.Common.Tool.Tools;
using Consul;
using MathNet.Numerics.Distributions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Consul.IApplicationBuilderExtension
{
    public static class IApplicationBuilderExtension
    {
        public static IApplicationBuilder UseArsConsul(this IApplicationBuilder app, IConsulRegisterConfiguration option)
        {
            ConsulClient client = new ConsulClient(
                new ConsulClientConfiguration
                {
                    Address = new Uri(option.ConsulAddress),
                    Datacenter = "dc1"
                }, CreateHttpClient(option.UseHttps,app.ApplicationServices));

            IConfiguration configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();
            string ip = option.ServiceIp!;
            int port = option.ServicePort.GetValueOrDefault();

            string serviceId = $"service:{ip}:{port}";//服务ID，每个服务都是唯一的

            var check = new AgentServiceCheck()
            {
                Interval = TimeSpan.FromSeconds(5),//多久检查一次心跳
                Timeout = TimeSpan.FromSeconds(10),//超时时间
                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5) //服务停止多久后注销服务
            };
            if (string.IsNullOrEmpty(option.HttpHealthAction))
            {
                check.GRPC = $"{ip}:{port}";
                if (option.UseHttps) 
                {
                    check.TLSSkipVerify = true;
                    check.GRPCUseTLS = true;
                }
            }
            else 
            {
                if (option.UseHttps)
                {
                    check.TLSSkipVerify = true;
                    check.HTTP = $"https://{ip}:{port}/{option.HttpHealthAction}";
                }
                else 
                {
                    check.HTTP = $"http://{ip}:{port}/{option.HttpHealthAction}";
                }
            }
            //服务注册
            client.Agent.ServiceRegister(new AgentServiceRegistration()
            {
                ID = serviceId, //唯一的
                Name = option.ServiceName, //组名称-Group
                Address = ip, //ip地址
                Port = port, //端口
                Tags = new string[] { option.ServiceName },
                Check = check,
                Meta = new Dictionary<string,string>
                { 
                    { "protocol", option.UseHttps ? "https": "http" }
                }
            }).Wait();

            //应用程序终止时,注销服务
            var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
            lifetime.ApplicationStopping.Register(() =>
            {
                client.Agent.ServiceDeregister(serviceId).Wait();
            });
            return app;
        }

        private static HttpClient CreateHttpClient(bool useHttps,IServiceProvider serviceProvider)
        {
            IHttpClientProvider httpClientProvider = serviceProvider.GetRequiredService<IHttpClientProvider>();
            
            var httpclient = httpClientProvider.CreateClient(useHttps);

            httpclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            return httpclient;
        }
    }
}
