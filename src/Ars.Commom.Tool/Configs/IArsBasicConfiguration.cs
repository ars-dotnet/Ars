using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.Configs
{
    public interface IArsBasicConfiguration
    {
        /// <summary>
        /// application根目录
        /// </summary>
        string Root { get; }

        /// <summary>
        /// 程序访问的域名
        /// </summary>
        string AppAccessDomain { get; }

        /// <summary>
        /// 应用程序的ip/容器的ip 
        /// 默认127.0.0.1
        /// </summary>
        string Ip { get; }

        /// <summary>
        /// 应用程序的端口/容器的端口
        /// 默认5000
        /// </summary>
        int Port { get; } 
    }
}
