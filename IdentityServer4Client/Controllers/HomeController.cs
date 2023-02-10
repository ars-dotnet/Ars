using Ars.Common.Core.AspNetCore.OutputDtos;
using Ars.Common.Core.Configs;
using GrpcClients;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;
using System.Text;

namespace IdentityServer4Client.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class HomeController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;
        public HomeController(
            IHttpContextAccessor httpContextAccessor, 
            IHttpClientFactory httpClientFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("~/signin-oidc")]
        public IActionResult SignIn() 
        {

            return Json("SinIg");
        }

        [HttpGet]
        [Route("~/signout-oidc")]
        public IActionResult SignOut()
        {

            return Json("SignOut");
        }

        [HttpPost]
        public async Task<ArsOutput<LoginOutput>> AuthorizeCode()
        {
            if (_httpContextAccessor.HttpContext?.Request?.Headers?.TryGetValue("Authorization", out StringValues value) ?? false)
            {
                var m = value.ToString().Split(" ");
                string[]? cc = Encoding.UTF8.GetString(Convert.FromBase64String(m[1]))?.Split(":");
                if (cc?.Any() ?? false)
                {
                    using var httpclient = _httpClientFactory.CreateClient("http");
                    var tokenresponse = await httpclient.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
                    {
                        ClientId = cc[0],
                        //ClientSecret = cc[1],
                        
                        GrantType = "authorization_code",
                        Address = "http://127.0.0.1:5134/connect/authorize",
                        Code = "code",
                        RedirectUri = "http://127.0.0.1:5212/signin-oidc",
                        // optional PKCE parameter
                        CodeVerifier = "xyz"
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
        [Authorize]
        public async Task<IActionResult> Authorize()
        {
            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            // 获取idToken
            var idToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);
            // 获取刷新Token
            var refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            return Json("OK");
        }
    }
}
