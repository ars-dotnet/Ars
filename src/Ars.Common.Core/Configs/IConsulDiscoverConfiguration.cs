using Ars.Common.Core.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Configs
{
    public interface IConsulDiscoverConfiguration
    {
        IEnumerable<ConsulConfiguration> ConsulDiscovers { get; }
    }

    public interface IConsulConfiguration
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        string ServiceName { get; }

        /// <summary>
        /// consul服务的地址
        /// </summary>
        string ConsulAddress { get; }
    }

    public class ConsulConfiguration : IConsulConfiguration
    {
        /// <summary>
        /// 被调用的服务名称
        /// </summary>
        public string ServiceName { get; set; }

        public string ConsulAddress { get; set; }

        /// <summary>
        /// 通讯配置
        /// </summary>
        public CommunicationConfiguration Communication { get; set; }
    }

    public class CommunicationConfiguration : IArsCertificateConfiguration
    {
        /// <summary>
        /// 通讯方式
        /// </summary>
        public CommunicationWay CommunicationWay { get; set; }

        /// <summary>
        /// httpclient永不超时
        /// </summary>
        public bool IgnoreTimeOut { get; set; }

        /// <summary>
        /// grpc是否采用http1通讯
        /// </summary>
        public bool GrpcUseHttp1Protocol { get; set; }

        /// <summary>
        /// 是否采用https
        /// </summary>
        public bool UseHttps { get; set; }

        public string? CertificatePath { get; set; }

        public string? CertificatePassWord { get; set; }

        /// <summary>
        /// 是否采用identityServer4身份认证
        /// </summary>
        public bool UseIdentityServer4Valid { get; set; }

        /// <summary>
        /// identityserver4服务器是否使用https
        /// </summary>
        public bool IdentityServer4UseHttps { get; set; }

        /// <summary>
        /// identityserver4服务器地址
        /// </summary>
        public string IdentityServer4Address { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string Scope { get; set; }

        public string GrantType { get; set; }
    }

    [Flags]
    public enum CommunicationWay 
    {
        Both = 0,

        Grpc = 1,

        HttpClient = 2,
    }
}
