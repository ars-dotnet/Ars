using Ars.Commom.Tool.Extension;
using Ars.Common.SignalR.ClearCache;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.SignalR.Caches
{
    internal class MemoryHubCacheManager : BaseHubCacheManager
    {
        private static IDictionary<string, ConcurrentDictionary<string, SignalRCacheScheme>> _memoryCache;

        private readonly IMemoryHubHeartOverTimeListener _signalrHeartOverTimeListener;

        protected static int count = 0;

        public MemoryHubCacheManager(
            IMemoryHubHeartOverTimeListener signalrHeartOverTimeListener,
            ILoggerFactory loggerFactory) 
            : base(loggerFactory)
        {
            _memoryCache ??= new ConcurrentDictionary<string, ConcurrentDictionary<string, SignalRCacheScheme>>();

            _signalrHeartOverTimeListener = signalrHeartOverTimeListener;
        }

        /// <summary>
        /// 上线
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="connectionId"></param>
        /// <param name="signalRCacheScheme"></param>
        /// <returns></returns>
        public override async Task ClientOnConnection(string terminal, string connectionId, SignalRCacheScheme signalRCacheScheme)
        {
            logger.LogInformation($"signalr add connection by memorycache" +
                       $"【terminal:{terminal},connectionId:{connectionId}," +
                       $"signalRCacheScheme:{JsonConvert.SerializeObject(signalRCacheScheme)}】");

            await CheckIfOverTime(string.Empty);

            await AddOrUpdateCache(terminal, connectionId, signalRCacheScheme);
        }

        /// <summary>
        /// 下线
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="connectionId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public override Task ClientDisConnection(string terminal, string connectionId)
        {
            return Clear(terminal, connectionId);
        }

        /// <summary>
        /// 客户端是否在线
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public override ValueTask<bool> ClientIsOnline(string terminal, string connectionId)
        {
            bool online = false;
            if (_memoryCache.TryGetValue(GetCacheKey(terminal), out var cache))
            {
                if (cache.TryGetValue(connectionId, out var value) && 
                    (DateTime.Now - value.HeartTime).TotalSeconds <= disLineSecond)
                {
                    online = true;
                }
            }

            return new ValueTask<bool>(online);
        }

        /// <summary>
        /// 用户是否在线
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public override ValueTask<bool> UserIsOnline(string terminal, string userName)
        {
            bool online = false;
            if (_memoryCache.TryGetValue(GetCacheKey(terminal), out var cache))
            {
                if (cache.Values.Any(r =>
                    r.UserName.IsNotNullOrEmpty() &&
                    r.UserName!.Equals(userName) &&
                    (DateTime.Now - r.HeartTime).TotalSeconds <= disLineSecond))
                {
                    online = true;
                }
            }

            return new ValueTask<bool>(online);
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="connectionId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public override Task Refresh(string terminal, string connectionId)
        {
            return AddOrUpdateCache(terminal, connectionId, null);
        }

        /// <summary>
        /// 获取缓存项
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public override Task<SignalRCacheScheme?> GetCacheValue(string terminal, string connectionId)
        {
            if (connectionId.IsNotNullOrEmpty())
            {
                if (_memoryCache.TryGetValue(GetCacheKey(terminal), out var cache) &&
                    cache.TryGetValue(connectionId,out var value))
                {
                    return Task.FromResult<SignalRCacheScheme?>(value);
                }
            }

            return Task.FromResult<SignalRCacheScheme?>(null);
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        protected override Task Clear(string terminal, string connectionId)
        {
            if (connectionId.IsNotNullOrEmpty())
            {
                if (_memoryCache.TryGetValue(GetCacheKey(terminal), out var cache))
                {
                    cache.TryRemove(connectionId, out var _);
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// 添加、更新缓存
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="connectionId"></param>
        /// <param name="signalRCacheScheme"></param>
        /// <returns></returns>
        protected override Task AddOrUpdateCache(string terminal, string connectionId, SignalRCacheScheme? signalRCacheScheme)
        {
            if (_memoryCache.TryGetValue(GetCacheKey(terminal), out var cache))
            {
                if (cache.TryGetValue(connectionId, out var value))
                {
                    value.HeartTime = DateTime.Now;
                }
                else if(null != signalRCacheScheme)
                {
                    cache.TryAdd(connectionId, signalRCacheScheme!);
                }
            }
            else if(null != signalRCacheScheme)
            {
                var data = new ConcurrentDictionary<string, SignalRCacheScheme>();
                data.TryAdd(connectionId, signalRCacheScheme!);
                _memoryCache.TryAdd(GetCacheKey(terminal), data);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// 轮询检查有无过期的缓存
        /// </summary>
        /// <param name="terminal"></param>
        /// <returns></returns>
        protected override Task CheckIfOverTime(string terminal)
        {
            int tcount = Interlocked.Increment(ref count);

            if (1 == tcount)
            {
                _signalrHeartOverTimeListener.ListenSignalrHeartOverTime(_memoryCache, GetKey,disLineSecond);
            }

            return Task.CompletedTask;
        }
    }
}
