using Ars.Common.Core.IDependency;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Consul.GrpcHelper
{
    internal class GrpcClientProvider : IGrpcClientProvider,ISingletonDependency
    {
        private readonly ConsulHelper ConsulHelper;
        public GrpcClientProvider(ConsulHelper consulHelper)
        {
            ConsulHelper = consulHelper;
        }

        public async Task<T> GetGrpcClient<T>(string serviceName) where T : ClientBase<T>
        {
            string domain = await ConsulHelper.GetServiceDomain(serviceName,out bool useHttp1Protocol);
            return (T)Activator.CreateInstance(typeof(T), GetGrpcChannel(domain,useHttp1Protocol))!;
        }

        private GrpcChannel GetGrpcChannel(string domain,bool useHttp1Protocol) 
        {
            if (useHttp1Protocol)
            {
                return GrpcChannel.ForAddress(domain,new GrpcChannelOptions { HttpClient = GetHttpClient() });
            }
            else 
            {
                return GrpcChannel.ForAddress(domain);
            }
        }

        private HttpClient GetHttpClient() 
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
                Timeout = TimeSpan.FromDays(10),
            };

            return httpclient;
        }
    }
}
