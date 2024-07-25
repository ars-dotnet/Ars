using Ars.Commom.Tool.Extension;
using Ars.Common.Redis;
using Ars.Common.SignalR.Caches;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.SignalR.ClearCache
{
    public class RedisHubHeartOverTimeListener : IRedisHubHeartOverTimeListener,IDisposable
    {
        private Timer? _timer;

        private CancellationTokenSource? _cancellationTokenSource;

        private readonly IServiceProvider _serviceProvider;

        private string _cacheName;

        private HashSet<string> _keys;

        private int? _disLineSecond;

        private Func<string, string> _getKey;

        protected ILogger logger;

        public RedisHubHeartOverTimeListener(IServiceScopeFactory serviceScopeFactory,ILoggerFactory loggerFactory)
        {
            _keys = new HashSet<string>();

            _serviceProvider = serviceScopeFactory.CreateScope().ServiceProvider;

            logger = loggerFactory.CreateLogger(GetType());
        }

        public void AddKeys(string key)
        {
            _keys.Add(key);
        }

        public void ListenSignalrHeartOverTime(string cacheName, Func<string, string> getKey, int disLineSecond)
        {
            if (null != _cancellationTokenSource)
                return;

            _cancellationTokenSource = new CancellationTokenSource();

            _cacheName = cacheName;

            _getKey ??= getKey;

            _disLineSecond ??= disLineSecond;

            _timer ??= new Timer(Executing, _cancellationTokenSource, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));
        }

        protected virtual async void Executing(object? state) 
        {
            IArsHCacheProvider? _arsHCacheProvider = null;

            IEnumerable<IHubDisconnection>? _hubDisconnections = null;

            foreach (var key in _keys)
            {
                _arsHCacheProvider ??= _serviceProvider.GetRequiredService<IArsHCacheProvider>();

                var fileds = await _arsHCacheProvider.GetArsCache(_cacheName).HKeysAsync(key);

                foreach (var field in fileds)
                {
                    var value = await _arsHCacheProvider.GetArsCache(_cacheName).HGetAsync<SignalRCacheScheme>(key, field);
                    
                    if (((DateTime.Now - value?.HeartTime)?.TotalSeconds ?? 0) > _disLineSecond)
                    {
                        await _arsHCacheProvider.GetArsCache(_cacheName).HDelAsync(key, field);

                        _hubDisconnections ??= _serviceProvider.GetRequiredService<IEnumerable<IHubDisconnection>>();

                        if (_hubDisconnections.HasValue())
                        {
                            try
                            {
                                foreach (var hub in _hubDisconnections)
                                {
                                    await hub.ClientDisConnectionNoticeAysnc(_getKey(key), field, value!);
                                }
                            }
                            catch { }
                        }

                        logger.LogInformation("客户端:{0}连接到期，清除在线信息", field);
                    }
                }
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();

            _timer?.Dispose();

            _keys.Clear();
        }
    }
}
