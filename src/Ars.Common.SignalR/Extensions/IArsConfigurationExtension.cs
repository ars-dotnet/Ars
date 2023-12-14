using Ars.Common.Core.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.SignalR.Extensions
{
    public static class IArsConfigurationExtension
    {
        /// <summary>
        /// 如果采用redis做缓存，则需要注入redis组件arsConfiguration.AddArsRedis()
        /// </summary>
        /// <param name="arsConfiguration"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IArsConfiguration AddArsSignalR(this IArsConfiguration arsConfiguration, Action<ArsSignalRConfiguration>? action = null) 
        {
            return arsConfiguration.AddArsServiceExtension(new ArsSignalRServiceExtension(action));
        }
    }
}
