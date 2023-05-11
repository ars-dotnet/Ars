using Ars.Commom.Tool.Extension;
using Ars.Common.Redis;
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
        private static int count = 0;
        private readonly IArsHCacheProvider _arsHCacheProvider;
        private static HashSet<string>? _keys;
        public RedisHubCacheManager(IArsHCacheProvider arsHCacheProvider)
        {
            _arsHCacheProvider = arsHCacheProvider;
            _keys ??= new HashSet<string>();
        }

        protected override Task CheckIfOverTime(string key)
        {
            var tcount = Interlocked.Increment(ref count);
            _keys!.Add(GetCacheKey(key));

            if (1 == tcount)
            {
                Task.Run(async () =>
                {
                    while (true) 
                    {
                        foreach (var key in _keys) 
                        {
                            var fileds = await _arsHCacheProvider.GetArsCache(CacheName).HKeysAsync(key);
                            foreach (var field in fileds) 
                            {
                                var value = await _arsHCacheProvider.GetArsCache(CacheName).HGetAsync<SignalRCacheScheme>(key, field);
                                if (((DateTime.Now - value?.HeartTime)?.TotalSeconds ?? 0) > 30) 
                                {
                                    await _arsHCacheProvider.GetArsCache(CacheName).HDelAsync(key,fileds);
                                }
                            }
                        }

                        await Task.Delay(1000 * 60);
                    }
                });
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// 上线
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="connectionId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override async Task ClientOnConnection(string terminal, string connectionId, string? userId = null)
        {
            await CheckIfOverTime(terminal);

            await AddOrUpdateCache(terminal, connectionId, userId);
        }

        /// <summary>
        /// 下线
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override Task ClientDisConnection(string terminal, string connectionId, string? userId = null)
        {
            if (userId.IsNotNullOrEmpty())
            {
                return Clear(terminal, userId: userId);
            }
            return Clear(terminal, connectionId: connectionId);
        }

        /// <summary>
        /// 客户端是否在线
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override ValueTask<bool> ClientIsOnline(string terminal, string connectionId)
        {
            return CheckIfOnline(terminal, connectionId);
        }

        /// <summary>
        /// 用户是否在线
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override ValueTask<bool> UserIsOnline(string terminal, string userId)
        {
            return CheckIfOnline(terminal, userId);
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override Task Refresh(string terminal, string connectionId, string? userId = null)
        {
            return AddOrUpdateCache(terminal, connectionId, userId);
        }

        /// <summary>
        /// 清除
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected override Task Clear(string terminal, string connectionId = "", string? userId = null)
        {
            if (userId.IsNotNullOrEmpty())
            {
                return _arsHCacheProvider.GetArsCache(CacheName).HDelAsync(GetCacheKey(terminal), userId!);
            }

            if (connectionId.IsNotNullOrEmpty())
            {
                return _arsHCacheProvider.GetArsCache(CacheName).HDelAsync(GetCacheKey(terminal), connectionId);
            }

            return Task.CompletedTask;
        }

        private Task AddOrUpdateCache(string terminal, string connectionId = "", string? userId = null)
        {
            if (userId.IsNotNullOrEmpty())
            {
                return _arsHCacheProvider
                .GetArsCache(CacheName)
                .HSetAsync(
                    GetCacheKey(terminal),
                    userId!,
                    new SignalRCacheScheme { Type = "用户", HeartTime = DateTime.Now });
            }

            if (connectionId.IsNotNullOrEmpty())
            {
                return _arsHCacheProvider
                .GetArsCache(CacheName)
                .HSetAsync(
                    GetCacheKey(terminal),
                    connectionId,
                    new SignalRCacheScheme { Type = "客户端", HeartTime = DateTime.Now });
            }

            return Task.CompletedTask;
        }

        private async ValueTask<bool> CheckIfOnline(string terminal, string cid)
        {
            var data = await _arsHCacheProvider
                .GetArsCache(CacheName)
                .HGetAsync<SignalRCacheScheme>(GetCacheKey(terminal), cid);
            if (null == data)
                return false;

            if ((DateTime.Now - data.HeartTime).TotalSeconds > 30)
                return false;

            return true;
        }
    }
}
