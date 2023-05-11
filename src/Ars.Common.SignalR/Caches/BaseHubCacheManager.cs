using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.SignalR.Caches
{
    public abstract class BaseHubCacheManager : IHubCacheManager
    {
        public virtual string CacheName => "ars.signalr";

        protected virtual string GetCacheKey(string key)
        {
            return $"online.{key}";
        }

        public abstract Task ClientOnConnection(string terminal, string connectionId, string userId = "");

        public abstract Task ClientDisConnection(string terminal, string connectionId, string userId = "");

        public abstract ValueTask<bool> UserIsOnline(string terminal, string userId);

        public abstract ValueTask<bool> ClientIsOnline(string terminal, string connectionId);

        public abstract Task Refresh(string terminal, string connectionId = "", string userId = "");

        protected abstract Task Clear(string terminal, string connectionId = "", string? userId = null);

        /// <summary>
        /// 轮询检查有无过期的缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected virtual Task CheckIfOverTime(string key) 
        {
            return Task.CompletedTask;
        }
    }
}
