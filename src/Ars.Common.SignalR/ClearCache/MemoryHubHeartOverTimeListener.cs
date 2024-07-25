using Ars.Commom.Tool.Extension;
using Ars.Common.SignalR.Caches;
using Ars.Common.Tool.Extension;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.SignalR.ClearCache
{
    public class MemoryHubHeartOverTimeListener : IMemoryHubHeartOverTimeListener, IDisposable
    {
        private Timer? _timer;

        private CancellationTokenSource? _cancellationTokenSource;

        private readonly IServiceProvider _serviceProvider;

        protected int? _disLineSecond;

        protected Func<string, string> _getKey;

        protected IDictionary<string, ConcurrentDictionary<string, SignalRCacheScheme>> _memoryCache;

        public MemoryHubHeartOverTimeListener(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceProvider = serviceScopeFactory.CreateScope().ServiceProvider;
        }

        public void ListenSignalrHeartOverTime(
            IDictionary<string, ConcurrentDictionary<string, SignalRCacheScheme>> memoryCache,
            Func<string, string> getKey,
            int disLineSecond)
        {
            if (null != _cancellationTokenSource)
                return;

            _cancellationTokenSource = new CancellationTokenSource();

            _disLineSecond ??= disLineSecond;

            _getKey ??= getKey;

            _memoryCache ??= memoryCache;

            _timer ??= new Timer(Executing, _cancellationTokenSource, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();

            _timer?.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        protected virtual async void Executing(object? state)
        {
            if (!(state is CancellationTokenSource source) || source.Token.IsCancellationRequested)
                return;

            DateTime time = DateTime.Now;

            ConcurrentDictionary<string, SignalRCacheScheme> values;

            IEnumerable<IHubDisconnection>? _hubDisconnections = null;

            IHubCacheManager? hubCacheManager = null;

            foreach (var cache in _memoryCache)
            {
                values = cache.Value;

                foreach (var value in values.Where(r => (time - r.Value.HeartTime).TotalSeconds > _disLineSecond))
                {
                    values.TryRemove(value.Key, out var _);

                    _hubDisconnections ??= _serviceProvider.GetService<IEnumerable<IHubDisconnection>?>();

                    if (_hubDisconnections.HasValue())
                    {
                        try
                        {
                            hubCacheManager ??= _serviceProvider.GetRequiredService<IHubCacheManager>();

                            foreach (var hub in _hubDisconnections!)
                            {
                                await hub.ClientDisConnectionNoticeAysnc(_getKey(cache.Key), value.Key, value.Value);
                            }
                        }
                        catch { }
                    }
                }
            }

        }
    }
}
