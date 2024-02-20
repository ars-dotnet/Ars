using Ars.Common.Core.IDependency;
using ArsWebApiService.Controllers.BaseControllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ArsWebApiService.Controllers
{
    public class ApolloController : ArsWebApiBaseController
    {
        [Autowired]
        public IConfiguration Configuration { get; set; }

        /// <summary>
        /// 从apollo读取配置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetConfig() 
        {
            string timeout = Configuration.GetSection("timeout").Get<string>();

            return Ok(timeout);
        }
    }
}
