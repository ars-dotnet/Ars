using Ars.Commom.Tool.Certificates;
using Ars.Commom.Tool.Extension;
using Ars.Common.Core.Configs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Host.Extension
{
    public static class IWebHostBuilderExtension
    {
        public static IWebHostBuilder UseArsKestrel(this IWebHostBuilder webHostBuilder,IConfiguration configuration, Action<ListenOptions>? configure = null) 
        {
            webHostBuilder.UseKestrel(kestrel =>
            {
                var basicfg = configuration.GetSection(nameof(ArsBasicConfiguration)).Get<ArsBasicConfiguration>();

                if (null == basicfg)
                    throw new ArgumentNullException("appsettings => ArsBasicConfiguration not be null");

                if (basicfg.ServiceIp.IsNullOrEmpty())
                    throw new ArgumentNullException("appsettings => ArsBasicConfiguration.ServiceIp not be null");

                if (null == basicfg.ServicePort || 0 == basicfg.ServicePort)
                    throw new ArgumentNullException("appsettings => ArsBasicConfiguration.ServicePort not be null or zero");

                kestrel.Listen(IPAddress.Parse(basicfg.ServiceIp!), basicfg.ServicePort.Value,option => 
                {
                    if (null != configure)
                    {
                        configure(option);
                    }
                    else if (basicfg.UseHttps) 
                    {
                        option.Protocols = HttpProtocols.Http1AndHttp2;
                        option.UseHttps(
                            Certificate.Get(
                                basicfg.CertificatePath!, basicfg.CertificatePassWord!));
                    }
                });
            });

            return webHostBuilder;
        }

        public static IWebHostBuilder UseArsKestrelListenAnyIP(this IWebHostBuilder webHostBuilder, IConfiguration configuration, Action<ListenOptions>? configure = null)
        {
            webHostBuilder.UseKestrel(kestrel =>
            {
                var basicfg = configuration.GetSection(nameof(ArsBasicConfiguration)).Get<ArsBasicConfiguration>();

                if (null == basicfg)
                    throw new ArgumentNullException("appsettings => ArsBasicConfiguration not be null");

                if (null == basicfg.ServicePort || 0 == basicfg.ServicePort)
                    throw new ArgumentNullException("appsettings => ArsBasicConfiguration.ServicePort not be null or zero");

                kestrel.ListenAnyIP(basicfg.ServicePort.Value, option =>
                {
                    if (null != configure)
                    {
                        configure(option);
                    }
                    else if (basicfg.UseHttps)
                    {
                        option.Protocols = HttpProtocols.Http1AndHttp2;
                        option.UseHttps(
                            Certificate.Get(
                                basicfg.CertificatePath!, basicfg.CertificatePassWord!));
                    }
                });
            });

            return webHostBuilder;
        }
    }
}
