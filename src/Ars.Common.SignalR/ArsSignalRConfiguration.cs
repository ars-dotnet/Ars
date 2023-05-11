using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.SignalR
{
    public class ArsSignalRConfiguration
    {
        /// <summary>
        /// 缓存类型
        /// 0 内存缓存
        /// 1 redis缓存
        /// </summary>
        public int CacheType { get; set; }

        /// <summary>
        /// 是否使用messagepack中心协议
        /// </summary>
        public bool UseMessagePackProtocol { get; set; }
    }
}
