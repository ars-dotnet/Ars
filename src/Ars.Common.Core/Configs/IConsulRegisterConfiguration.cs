using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Configs
{
    public interface IConsulRegisterConfiguration : IConsulConfiguration
    {
        string ServiceIp { get; }

        int ServicePort { get; }

        string HttpHealthAction { get; }

        /// <summary>
        /// 是否采用https
        /// </summary>
        bool UseHttps { get; }

        string CertificatePath { get; }

        string CertificatePassWord { get; }
    }
}
