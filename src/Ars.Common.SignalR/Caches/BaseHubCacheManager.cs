using Ars.Common.SignalR.ClearCache;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.SignalR.Caches
{
    public abstract class BaseHubCacheManager : IHubCacheManager
    {
        protected readonly ILogger logger;

        /// <summary>
        /// 下线时间(秒)
        /// </summary>
        protected int disLineSecond;

        public BaseHubCacheManager( ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger(GetType());

            disLineSecond = 30;
        }

        public virtual string CacheName => "ars.signalr";

        public virtual string CacheKeyPrefixx => "online.";

        /// <summary>
        /// 上线
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="connectionId"></param>
        /// <param name="signalRCacheScheme"></param>
        /// <returns></returns>
        public abstract Task ClientOnConnection(string terminal, string connectionId, SignalRCacheScheme signalRCacheScheme);

        /// <summary>
        /// 下线
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public abstract Task ClientDisConnection(string terminal, string connectionId);

        /// <summary>
        /// 用户是否在线
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public abstract ValueTask<bool> UserIsOnline(string terminal, string userName);

        /// <summary>
        /// 客户端是否在线
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public abstract ValueTask<bool> ClientIsOnline(string terminal, string connectionId);
        
        /// <summary>
        /// 刷新缓存时间
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public abstract Task Refresh(string terminal, string connectionId);

        /// <summary>
        /// 获取缓存项
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public abstract Task<SignalRCacheScheme?> GetCacheValue(string terminal, string connectionId);

        /// <summary>
        /// 获取缓存key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected virtual string GetCacheKey(string key)
        {
            return $"{CacheKeyPrefixx}{key}";
        }

        /// <summary>
        /// 获取原key
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        protected virtual string GetKey(string cacheKey)
        {
            return cacheKey.Replace(CacheKeyPrefixx, "");
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        protected abstract Task Clear(string terminal, string connectionId);

        /// <summary>
        /// 添加、更新缓存
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="connectionId"></param>
        /// <param name="signalRCacheScheme"></param>
        /// <returns></returns>
        protected abstract Task AddOrUpdateCache(string terminal, string connectionId, SignalRCacheScheme? signalRCacheScheme);

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
