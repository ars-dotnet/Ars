using Ars.Common.Core.IDependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.SignalR.ClearCache
{
    public interface IRedisHubHeartOverTimeListener : ISingletonDependency
    {
        /// <summary>
        /// signalr客户端心跳超时检测
        /// </summary>
        /// <param name="cacheName"></param>
        /// <param name="getKey"></param>
        /// <param name="disLineSecond"></param>
        void ListenSignalrHeartOverTime(string cacheName, Func<string, string> getKey,int disLineSecond);

        /// <summary>
        /// 添加key
        /// </summary>
        /// <param name="key"></param>
        void AddKeys(string key);
    }
}
