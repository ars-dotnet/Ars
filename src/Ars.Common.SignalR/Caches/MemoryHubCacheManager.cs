using Ars.Commom.Tool.Extension;
using Microsoft.Extensions.Caching.Memory;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.SignalR.Caches
{
    internal class MemoryHubCacheManager : BaseHubCacheManager
    {
        private static IDictionary<string, ConcurrentDictionary<string, SignalRCacheScheme>> _memoryCache;
        private static int count = 0;
        public MemoryHubCacheManager()
        {
            _memoryCache ??= new ConcurrentDictionary<string, ConcurrentDictionary<string, SignalRCacheScheme>>();
        }

        protected override Task CheckIfOverTime(string key)
        {
            int tcount = Interlocked.Increment(ref count);
            if (1 == tcount)
            {
                Task.Run(async () =>
                {
                    while (true)
                    {
                        DateTime time = DateTime.Now;
                        foreach (var cache in _memoryCache.Values)
                        {
                            var keys = cache.Where(r => (time - r.Value.HeartTime).TotalSeconds > 30).Select(r => r.Key);
                            foreach (var key in keys)
                            {
                                cache.TryRemove(key, out var _);
                            }
                        }

                        await Task.Delay(1000 * 60);
                    }
                });
            }  
            
            return Task.CompletedTask;
        }

        public override async Task ClientOnConnection(string terminal, string connectionId, string? userId = null)
        {
            await CheckIfOverTime(string.Empty);

            await AddOrUpdate(terminal, connectionId, userId);
        }

        public override Task ClientDisConnection(string terminal, string connectionId, string? userId = null)
        {
            return Clear(terminal, connectionId, userId);
        }

        public override ValueTask<bool> ClientIsOnline(string terminal, string connectionId)
        {
            return CheckIfOnline(terminal, connectionId);
        }

        public override ValueTask<bool> UserIsOnline(string terminal, string userId)
        {
            return CheckIfOnline(terminal, userId);
        }

        public override Task Refresh(string terminal, string connectionId = "", string? userId = null)
        {
            return AddOrUpdate(terminal, connectionId, userId);
        }

        protected override Task Clear(string terminal, string connectionId = "", string? userId = null)
        {
            if (userId.IsNotNullOrEmpty())
            {
                if (_memoryCache.TryGetValue(GetCacheKey(terminal), out var cache))
                {
                    cache.TryRemove(userId!, out var _);
                }
            }

            if (connectionId.IsNotNullOrEmpty())
            {
                if (_memoryCache.TryGetValue(GetCacheKey(terminal), out var cache))
                {
                    cache.TryRemove(connectionId, out var _);
                }
            }

            return Task.CompletedTask;
        }

        private Task AddOrUpdate(string terminal, string connectionId = "", string? userId = null)
        {
            if (userId.IsNotNullOrEmpty())
            {
                return AddOrUpdateCache(terminal, userId!, "用户");
            }

            if (connectionId.IsNotNullOrEmpty())
            {
                return AddOrUpdateCache(terminal, connectionId, "客户端");
            }

            return Task.CompletedTask;
        }

        private Task AddOrUpdateCache(string terminal, string cid, string type)
        {
            if (_memoryCache.TryGetValue(GetCacheKey(terminal), out var cache))
            {
                if (cache.TryGetValue(cid, out var value))
                {
                    value.HeartTime = DateTime.Now;
                }
                else
                {
                    cache.TryAdd(cid, new SignalRCacheScheme { Type = type, HeartTime = DateTime.Now });
                }
            }
            else
            {
                var data = new ConcurrentDictionary<string, SignalRCacheScheme>();
                data.TryAdd(cid, new SignalRCacheScheme { Type = type, HeartTime = DateTime.Now });
                _memoryCache.TryAdd(GetCacheKey(terminal), data);
            }

            return Task.CompletedTask;
        }

        private ValueTask<bool> CheckIfOnline(string terminal, string cid)
        {
            bool online = false;
            if (_memoryCache.TryGetValue(GetCacheKey(terminal), out var cache))
            {
                if (cache.TryGetValue(cid, out var value))
                {
                    if ((DateTime.Now - value.HeartTime).TotalSeconds <= 30)
                    {
                        online = true;
                    }
                }

                goto over;
            }

        over:
            return new ValueTask<bool>(online);
        }
    }
}
