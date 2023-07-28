using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.Configs
{
    public interface IArsBasicConfiguration: IArsIpPortConfiguration, IArsCertificateConfiguration
    {
        /// <summary>
        /// application根目录
        /// </summary>
        string Root { get; }

        /// <summary>
        /// 程序访问的域名
        /// </summary>
        string AppAccessDomain { get; }
    }
}
