using Ars.Common.SignalR.Caches;
using Ars.Common.SignalR.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ArsWebApiService.Hubs
{
    //[Authorize]
    public class MyWebHub : ArsWebHub
    {
        public MyWebHub(
            IHubContext<MyWebHub> hubContext, 
            IHubCacheManager cacheManager, 
            ILoggerFactory loggerFactory,
            IEnumerable<IHubDisconnection> hubDisconnections) 
            : base(hubContext, cacheManager, loggerFactory, hubDisconnections)
        {

        }

        public override int Order => 1;
    }
}
