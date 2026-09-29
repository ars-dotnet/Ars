using AgileConfig.Client;
using Ars.Common.Core.IDependency;
using ArsWebApiService.Controllers.BaseControllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MyApiWithIdentityServer4;
using System.Runtime.InteropServices;

namespace ArsWebApiService.Controllers
{
    public class ApolloController : ArsWebApiBaseController
    {
        [Autowired]
        public IConfiguration Configuration { get; set; }

        private readonly IOptionsMonitor<ConfigOptions> _options;

        private readonly IConfigClient _configClient;

        public ApolloController(IOptionsMonitor<ConfigOptions> options, IConfigClient configClient)
        {
            _options = options;
            _configClient = configClient;
        }

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

        /// <summary>
        /// 从agileconfig读取配置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAgileConfig() 
        {
            var mx = Configuration["arsconfig:Test"];

            _configClient.Data.TryGetValue("arsconfig:Test",out var value);

            var a = _options.CurrentValue;

            return Ok((mx, value, a));
        }
    }
}
