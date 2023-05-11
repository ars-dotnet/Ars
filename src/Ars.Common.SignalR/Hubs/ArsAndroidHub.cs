using Ars.Common.SignalR.Caches;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.SignalR.Hubs
{
    public class ArsAndroidHub : BaseHub<ArsAndroidHub>
    {
        public ArsAndroidHub(
            IHubContext<ArsAndroidHub> hubContext, 
            IHubCacheManager cacheManager, 
            ILoggerFactory loggerFactory)
            : base(hubContext, cacheManager, loggerFactory)
        {

        }

        public override int Order => int.MaxValue;

        public override string Terminal => "android";
    }
}
