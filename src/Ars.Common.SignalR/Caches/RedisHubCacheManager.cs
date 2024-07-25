using Ars.Commom.Tool.Extension;
using Ars.Common.Redis;
using Ars.Common.SignalR.ClearCache;
using Microsoft.Extensions.Logging;
using NPOI.SS.Formula.PTG;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.SignalR.Caches
{
    internal class RedisHubCacheManager : BaseHubCacheManager
    {
        private readonly IArsHCacheProvider _arsHCacheProvider;

        private readonly IRedisHubHeartOverTimeListener _redisHubHeartOverTimeListener;

        protected static int count = 0;

        public RedisHubCacheManager(
            IArsHCacheProvider arsHCacheProvider,
            IRedisHubHeartOverTimeListener redisHubHeartOverTimeListener,
            ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
            _arsHCacheProvider = arsHCacheProvider;

            _redisHubHeartOverTimeListener = redisHubHeartOverTimeListener;
        }

        /// <summary>
        /// 上线
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="connectionId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public override async Task ClientOnConnection(string terminal, string connectionId, SignalRCacheScheme signalRCacheScheme)
        {
            await CheckIfOverTime(terminal);

            await AddOrUpdateCache(terminal, connectionId, signalRCacheScheme);
        }

        /// <summary>
        /// 下线
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
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
        /// <exception cref="NotImplementedException"></exception>
        public override async ValueTask<bool> ClientIsOnline(string terminal, string connectionId)
        {
            var data = await _arsHCacheProvider
                .GetArsCache(CacheName)
                .HGetAsync<SignalRCacheScheme>(GetCacheKey(terminal), connectionId);
            if (null == data)
                return false;

            if ((DateTime.Now - data.HeartTime).TotalSeconds > disLineSecond)
                return false;

            return true;
        }

        /// <summary>
        /// 用户是否在线
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override async ValueTask<bool> UserIsOnline(string terminal, string userName)
        {
            var data = await _arsHCacheProvider
                .GetArsCache(CacheName)
                .HGetAllAsync<SignalRCacheScheme>(GetCacheKey(terminal));
            if (null == data)
                return false;

            if (data.Values.Any(r =>
                r.UserName.IsNotNullOrEmpty() &&
                r.UserName!.Equals(userName) &&
                (DateTime.Now - r.HeartTime).TotalSeconds <= disLineSecond))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="connectionId"></param>
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
        public override async Task<SignalRCacheScheme?> GetCacheValue(string terminal, string connectionId) 
        {
            if (connectionId.IsNotNullOrEmpty())
            {
                return await _arsHCacheProvider
                    .GetArsCache(CacheName)
                    .HGetAsync<SignalRCacheScheme>(
                        GetCacheKey(terminal), connectionId);
            }

            return null;
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
                return _arsHCacheProvider.GetArsCache(CacheName).HDelAsync(GetCacheKey(terminal), connectionId);
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
        protected override async Task AddOrUpdateCache(string terminal, string connectionId, SignalRCacheScheme? signalRCacheScheme)
        {
            if (connectionId.IsNotNullOrEmpty())
            {
                var data = await _arsHCacheProvider
                    .GetArsCache(CacheName)
                    .HGetAsync<SignalRCacheScheme>(
                        GetCacheKey(terminal),connectionId);

                //添加缓存
                if (null == data && null != signalRCacheScheme)
                {
                    await _arsHCacheProvider
                    .GetArsCache(CacheName)
                    .HSetAsync(
                        GetCacheKey(terminal),
                        connectionId,
                        signalRCacheScheme!);
                }
                
                //更新缓存
                if (null != data) 
                {
                    data.HeartTime = DateTime.Now;

                    await _arsHCacheProvider
                    .GetArsCache(CacheName)
                    .HSetAsync(
                        GetCacheKey(terminal),
                        connectionId,
                        data);
                }
            }

            return;
        }

        /// <summary>
        /// 轮询检查有无过期的缓存
        /// </summary>
        /// <param name="terminal"></param>
        /// <returns></returns>
        protected override Task CheckIfOverTime(string terminal)
        {
            var tcount = Interlocked.Increment(ref count);

            _redisHubHeartOverTimeListener.AddKeys(GetCacheKey(terminal));

            if (1 == tcount)
            {
                _redisHubHeartOverTimeListener.ListenSignalrHeartOverTime(CacheName,GetKey,disLineSecond);
            }

            return Task.CompletedTask;
        }
    }
}
