using Ars.Common.Core.AspNetCore.OutputDtos;
using Ars.Common.Core.Configs;
using GrpcClients.Models;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Text;

namespace GrpcClients.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize]
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

        [HttpPost]
        [AllowAnonymous]
        public async Task<ArsOutput<LoginOutput>> ClientCredentials() 
        {
            if (_httpContextAccessor.HttpContext?.Request?.Headers?.TryGetValue("Authorization", out StringValues value) ?? false)
            {
                var m = value.ToString().Split(" ");
                string[]? cc = Encoding.UTF8.GetString(Convert.FromBase64String(m[1]))?.Split(":");
                if (cc?.Any() ?? false)
                {
                    using var httpclient = _httpClientFactory.CreateClient("http");
                    var tokenresponse = await httpclient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                    {
                        ClientId = cc[0],
                        ClientSecret = cc[1],
                        Scope = "grpcapi-scope",
                        GrantType = "client_credentials",
                        Address = $"{_clientConfiguration.Authority}/connect/token"
                    });

                    if (tokenresponse.IsError)
                        return ArsOutput<LoginOutput>.Failed(1, tokenresponse.Error);

                    var datas = JsonConvert.DeserializeObject<LoginOutput>(tokenresponse.Json.ToString())!;
                    return ArsOutput<LoginOutput>.Success(datas);
                }
            }

            return ArsOutput<LoginOutput>.Failed(1, "参数错误");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ArsOutput<LoginOutput>> Password([FromBody] LoginIn input)
        {
            if (_httpContextAccessor.HttpContext?.Request?.Headers?.TryGetValue("Authorization", out StringValues value) ?? false)
            {
                var m = value.ToString().Split(" ");
                string[]? cc = Encoding.UTF8.GetString(Convert.FromBase64String(m[1]))?.Split(":");
                if (cc?.Any() ?? false)
                {
                    using var httpclient = _httpClientFactory.CreateClient("http");
                    var tokenresponse = await httpclient.RequestPasswordTokenAsync(new PasswordTokenRequest
                    {
                        ClientId = cc[0],
                        ClientSecret = cc[1],
                        Scope = "grpcapi-scope",
                        GrantType = "password",
                        Address = $"{_clientConfiguration.Authority}/connect/token",
                        UserName = input.UserName,
                        Password = input.PassWord
                    });

                    if (tokenresponse.IsError)
                        return ArsOutput<LoginOutput>.Failed(1, tokenresponse.Error);

                    var datas = JsonConvert.DeserializeObject<LoginOutput>(tokenresponse.Json.ToString())!;
                    return ArsOutput<LoginOutput>.Success(datas);
                }
            }

            return ArsOutput<LoginOutput>.Failed(1, "参数错误");
        }

        [HttpPost]
        public IActionResult Authorize() 
        {
            return Json("Ok");
        }
    }
}
