using Ars.Common.SignalR.Caches;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.SignalR.Hubs
{
    public class ArsWebHub : BaseHub<ArsWebHub>
    {
        public ArsWebHub(
            IHubContext<ArsWebHub> hubContext,
            IHubCacheManager cacheManager,
            ILoggerFactory loggerFactory)
            : base(hubContext, cacheManager, loggerFactory)
        {

        }

        public override int Order => int.MaxValue;

        public override string Terminal => "web";
    }
}
