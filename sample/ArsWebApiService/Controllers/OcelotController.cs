using Ars.Common.Tool;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArsWebApiService.Controllers
{
    [Authorize("default")]
    public class OcelotController : ArsWebApiBaseController
    {
        /// <summary>
        /// 测试TimeoutRejectedException
        /// 超时状态码504
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> TimeoutRejectedException() 
        {
            await Task.Delay(1000 * 60);

            return Ok();
        }

        /// <summary>
        /// 测试HttpRequestException
        /// 熔断、降级状态码503
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> HttpRequestException()
        {
            Valid.ThrowException("throw HttpRequestException");

            return Ok();
        }
    }
}
