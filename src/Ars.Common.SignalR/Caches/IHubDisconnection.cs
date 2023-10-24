using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.SignalR.Caches
{
    /// <summary>
    /// signalr客户端下线通知
    /// </summary>
    public interface IHubDisconnection
    {
        /// <summary>
        /// 下线通知
        /// </summary>
        /// <param name="terminal">终端</param>
        /// <param name="connectionId">客户端id</param>
        /// <param name="signalRCacheScheme">缓存信息</param>
        /// <returns></returns>
        Task ClientDisConnectionNoticeAysnc(string terminal, string connectionId, SignalRCacheScheme? signalRCacheScheme);
    }
}
