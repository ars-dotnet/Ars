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
        public string ServiceName { get; set; }

        public string ConsulAddress { get; set; }

        /// <summary>
        /// 是否用http1通讯
        /// </summary>
        public bool UseHttp1Protocol { get; set; }

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

    public interface IConsulDiscoverConfiguration
    {
        IEnumerable<ConsulConfiguration> ConsulDiscovers { get; }
    }
}
