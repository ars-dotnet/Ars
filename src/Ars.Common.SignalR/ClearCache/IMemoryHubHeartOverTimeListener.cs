using Ars.Common.Core.IDependency;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.SignalR.ClearCache
{
    public interface IMemoryHubHeartOverTimeListener : ISingletonDependency
    {
        /// <summary>
        /// signalr客户端心跳超时检测
        /// </summary>
        void ListenSignalrHeartOverTime(
            IDictionary<string, ConcurrentDictionary<string, SignalRCacheScheme>> memoryCache,
            Func<string, string> getKey,
            int disLineSecond);
    }
}
