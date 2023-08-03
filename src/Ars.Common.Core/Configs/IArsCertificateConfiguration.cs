using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Configs
{
    /// <summary>
    /// 证书配置
    /// </summary>
    public interface IArsCertificateConfiguration
    {
        string? CertificatePath { get; }

        string? CertificatePassWord { get; }
    }
}
