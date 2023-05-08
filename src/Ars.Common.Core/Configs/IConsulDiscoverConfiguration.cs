using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Configs
{
    public interface IConsulConfiguration
    {
        string ServiceName { get; }

        string ConsulAddress { get; }
    }

    public class ConsulConfiguration
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

    public interface IConsulDiscoverConfiguration
    {
        IEnumerable<ConsulConfiguration> ConsulDiscovers { get; }
    }

    public class CommunicationConfiguration 
    {
        /// <summary>
        /// 通讯方式
        /// </summary>
        public CommunicationWay CommunicationWay { get; set; }

        public bool IgnoreTimeOut { get; set; }

        /// <summary>
        /// grpc是否采用http1通讯
        /// </summary>
        public bool GrpcUseHttp1Protocol { get; set; }

        /// <summary>
        /// 是否采用https
        /// </summary>
        public bool UseHttps { get; set; }

        public string CertificatePath { get; set; }

        public string CertificatePassWord { get; set; }

        /// <summary>
        /// 是否采用identityServer4身份认证
        /// </summary>
        public bool UseIdentityServer4Valid { get; set; }

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
