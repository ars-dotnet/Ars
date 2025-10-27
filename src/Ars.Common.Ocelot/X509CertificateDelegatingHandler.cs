using Ars.Commom.Tool.Certificates;
using Ars.Commom.Tool.Extension;
using Ars.Common.Core.Configs;
using Microsoft.Extensions.Options;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Ars.Common.Ocelot
{
    /// <summary>
    /// https certificate
    /// </summary>
    public class X509CertificateDelegatingHandler : DelegatingHandler
    {
        private readonly IOptions<IArsBasicConfiguration> _arsBasicConfiguration;
        public X509CertificateDelegatingHandler(IOptions<IArsBasicConfiguration> arsBasicConfiguration)
        {
            _arsBasicConfiguration = arsBasicConfiguration;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, 
            CancellationToken cancellationToken)
        {
            if (null == _arsBasicConfiguration.Value)
                throw new ArgumentNullException("appsettings => ArsBasicConfiguration not be null");

            if(_arsBasicConfiguration.Value.CertificatePath.IsNullOrEmpty())
                throw new ArgumentNullException("appsettings => ArsBasicConfiguration.CertificatePath not be null");

            if (_arsBasicConfiguration.Value.CertificatePassWord.IsNullOrEmpty())
                throw new ArgumentNullException("appsettings => ArsBasicConfiguration.CertificatePassWord not be null");

            var inner = InnerHandler;
            while (inner is DelegatingHandler)
            {
                inner = ((DelegatingHandler)inner).InnerHandler;
            }

            // inner is HttpClientHandler
            //if (inner is HttpClientHandler httpClientHandler)
            //{
            //    if (httpClientHandler.ClientCertificateOptions != ClientCertificateOption.Automatic)
            //    {
            //        httpClientHandler.SslProtocols = SslProtocols.Tls12;

            //        httpClientHandler.ClientCertificates.Add(
            //            Certificate.Get(
            //                _arsBasicConfiguration.Value.CertificatePath!,
            //                _arsBasicConfiguration.Value.CertificatePassWord!)
            //            );

            //        httpClientHandler.ServerCertificateCustomValidationCallback =
            //            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            //        httpClientHandler.ClientCertificateOptions = ClientCertificateOption.Automatic;
            //    }
            //}

            if (inner is SocketsHttpHandler _)
            {
                HttpClientHandler httpClientHandler = new HttpClientHandler();

                httpClientHandler.SslProtocols = SslProtocols.Tls12;

                httpClientHandler.ClientCertificates.Add(
                    Certificate.Get(
                        _arsBasicConfiguration.Value.CertificatePath!,
                        _arsBasicConfiguration.Value.CertificatePassWord!)
                    );

                httpClientHandler.ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

                httpClientHandler.ClientCertificateOptions = ClientCertificateOption.Automatic;

                InnerHandler = httpClientHandler;
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
