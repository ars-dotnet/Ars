using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.Configs
{
    public interface IArsIpPortConfiguration
    {
        /// <summary>
        /// ip
        /// </summary>
        string? ServiceIp { get; }
        
        /// <summary>
        /// port
        /// </summary>
        int? ServicePort { get; }

        /// <summary>
        /// 是否采用https
        /// </summary>
        bool UseHttps { get; }
    }
}
