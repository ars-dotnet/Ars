using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.SignalR.Caches
{
    public interface IHubCacheManager
    {
        string CacheName { get; }

        /// <summary>
        /// 客户端上线添加缓存
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="connectionId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task ClientOnConnection(string terminal, string connectionId, string? userId = null);

        /// <summary>
        /// 客户端下线清除缓存
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="connectionId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task ClientDisConnection(string terminal, string connectionId, string? userId = null);

        /// <summary>
        /// 用户是否在线
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        ValueTask<bool> UserIsOnline(string terminal, string userId);

        /// <summary>
        /// 客户端是否在线
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        ValueTask<bool> ClientIsOnline(string terminal, string connectionId);

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="connectionId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task Refresh(string terminal, string connectionId, string? userId = null);
    }
}
