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
