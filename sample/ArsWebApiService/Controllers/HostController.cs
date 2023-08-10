using Ars.Common.Core.AspNetCore.HostService;
using Microsoft.AspNetCore.Mvc;

namespace ArsWebApiService.Controllers
{
    /// <summary>
    /// 手动调度
    /// </summary>
    public class HostController : ArsWebApiBaseController
    {
        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        [HttpPost]
        public Task StartAsync([FromServices]IArsManualExecutingManager manager) 
        {
            return manager.StartAsync();
        }

        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        [HttpPost]
        public Task StopAsync([FromServices] IArsManualExecutingManager manager) 
        {
            return manager.StopAsync();
        }
    }
}
