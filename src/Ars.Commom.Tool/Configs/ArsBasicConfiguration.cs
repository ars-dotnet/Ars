using Ars.Commom.Tool.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.Configs
{
    public class ArsBasicConfiguration : IArsBasicConfiguration
    {
        public ArsBasicConfiguration()
        {
            Root = AppDomain.CurrentDomain.BaseDirectory;
            Ip = "127.0.0.1";
            Port = 5000;
        }

        /// <summary>
        /// application根目录
        /// </summary>
        public string Root { get; }

        /// <summary>
        /// 程序访问的域名
        /// </summary>
        public string AppAccessDomain { get; set; }

        /// <summary>
        /// 应用程序的ip/容器的ip
        /// 默认127.0.0.1
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// 应用程序的端口/容器的端口
        /// 默认5000
        /// </summary>
        public int Port { get; set; }
    }
}
