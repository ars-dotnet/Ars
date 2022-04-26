using Ars.Commom.Host.Extension;
using Ars.Common.Consul;
using Ars.Common.Consul.IApplicationBuilderExtension;
using Ars.Common.Consul.Option;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcGreeter.greet;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrpcClient
{
    internal class Program
    {
        private static IServiceProvider serviceProvider;
        static Program()
        {
            var builder = WebApplication.CreateBuilder();
            var arsbuilder = builder.Services.AddArserviceCore(builder);
            arsbuilder.AddArsConsulDiscover(option =>
            {
                option.ConsulDiscovers = new List<ConsulDiscover>
                {
                    new ConsulDiscover
                    {
                        ConsulAddress = "http://127.0.0.1:8500",
                        ServiceName = "apigrpc"
                    }
                };
            });
            serviceProvider = arsbuilder.Services.Provider;
        }

        static async Task Main(string[] args)
        {
            var client = await Get<Greeter.GreeterClient>();
            var res = await client.SayHelloAsync(new HelloRequest
            {
                Name = "test"
            });

            var a = await client.TestAsync(new TestInput());

            Console.WriteLine($"{res.Message}");
            Console.Read();
        }

        static async Task<T> Get<T>()
            where T : ClientBase<T>
        {
            ConsulHelper consulHelper = serviceProvider.GetRequiredService<ConsulHelper>();
            string url = await consulHelper.GetServiceDomain("apigrpc");
            var channel = GrpcChannel.ForAddress(url);
            return (T)Activator.CreateInstance(typeof(T), channel);
        }
    }
}
