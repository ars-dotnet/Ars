using Ars.Common.Core.AspNetCore.OutputDtos;
using Ars.Common.Core.Configs;
using GrpcClients.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Text;

namespace GrpcClients.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class IdentityServer4Controller : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IArsIdentityClientConfiguration _clientConfiguration;
        public IdentityServer4Controller(
            IHttpContextAccessor httpContextAccessor,
            IHttpClientFactory httpClientFactory,
            IArsIdentityClientConfiguration clientConfiguration)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
            _clientConfiguration = clientConfiguration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


    }
}
