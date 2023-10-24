using Ars.Common.Core.IDependency;
using Ars.Common.SignalR;
using Ars.Common.SignalR.Caches;

namespace ArsWebApiService
{
    public class HubDisconnection1 : IHubDisconnection, IScopedDependency
    {
        public Task ClientDisConnectionNoticeAysnc(string terminal, string connectionId, SignalRCacheScheme? signalRCacheScheme)
        {
            throw new NotImplementedException();
        }
    }

    public class HubDisconnection2 : IHubDisconnection, IScopedDependency
    {
        public Task ClientDisConnectionNoticeAysnc(string terminal, string connectionId, SignalRCacheScheme? signalRCacheScheme)
        {
            throw new NotImplementedException();
        }
    }
}
