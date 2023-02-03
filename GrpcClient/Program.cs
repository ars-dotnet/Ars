using Ars.Commom.Host.Extension;
using Ars.Common.Consul;
using Ars.Common.Consul.IApplicationBuilderExtension;
using Ars.Common.Consul.Option;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using GrpcGreeter.greet;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace GrpcClient
{
    internal class Program
    {
        private static IServiceProvider serviceProvider;
        static Program()
        {
            var builder = WebApplication.CreateBuilder();
            builder.Host.ConfigureAppConfiguration(option =>
            {
                option.AddJsonFile("appsetting.json",true,true);
            });
            var arsbuilder = builder.Services.AddArserviceCore(builder.Host);
            arsbuilder.AddArsConsulDiscoverClient();
            serviceProvider = arsbuilder.Services.Provider;
        }

        static async Task Main(string[] args)
        {
            var client = await Get<Greeter.GreeterClient>("apigrpc");
            var res = await client.SayHelloAsync(new HelloRequest
            {
                Name = "test"
            });

            Console.WriteLine($"{res.Message}");
            Console.Read();
        }

        //static async Task Main(string[] args)
        //{
        //    //var channel = GrpcChannel.ForAddress("http://localhost:7903");
        //    var channel = GrpcChannel.ForAddress("http://127.0.0.1:5134");
        //    Greeter.GreeterClient client = new Greeter.GreeterClient(channel);
        //    //var channel = GrpcChannel.ForAddress(url);
        //    //var client = await Get<Greeter.GreeterClient>("apigrpc");
        //    var res = await client.SayHelloAsync(new HelloRequest
        //    {
        //        Name = "test"
        //    });

        //    Console.WriteLine($"{res.Message}");
        //    Console.Read();
        //}

        static async Task<T> Get<T>(string serviceName)
            where T : ClientBase<T>
        {
            ConsulHelper consulHelper = serviceProvider.GetRequiredService<ConsulHelper>();
            string url = await consulHelper.GetServiceDomain(serviceName,out bool _);
            var channel = GrpcChannel.ForAddress(url,new GrpcChannelOptions() { HttpClient = CreateHttpClient()});
            return (T)Activator.CreateInstance(typeof(T), channel);
        }

        private static HttpClient CreateHttpClient()
        {
            var handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                SslProtocols = SslProtocols.Tls12,
            };

            // var path = Path.Combine(Directory.GetCurrentDirectory(), "cert", "client.pfx");
            // //加载客户端证书
            // var crt = new X509Certificate2(path, "123456");
            // handler.ClientCertificates.Add(crt);

            handler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            var grpchandler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, handler)//https://github.com/grpc/grpc-dotnet/issues/1110
            {
                HttpVersion = new Version(1, 1)
            };

            var httpclient = new HttpClient(grpchandler)
            {
                Timeout = TimeSpan.FromMinutes(5),
            };

            return httpclient;
        }
    }
}
