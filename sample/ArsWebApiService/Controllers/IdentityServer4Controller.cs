using Ars.Common.Core.AspNetCore.OutputDtos;
using Ars.Common.Core.Configs;
using Ars.Common.Core.IDependency;
using Ars.Common.Tool.Tools;
using ArsWebApiService.Controllers.BaseControllers;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using MyApiWithIdentityServer4.Dtos;
using Newtonsoft.Json;
using System.Text;

namespace ArsWebApiService.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class IdentityServer4Controller : MyControllerBase
    {
        [Authorize("default")]
        [HttpGet]
        public IActionResult GetClaims()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ArsOutput<LoginOutput>> ClientCredentials()
        {
            if (HttpContextAccessor.HttpContext?.Request?.Headers?.TryGetValue("Authorization", out StringValues value) ?? false)
            {
                var m = value.ToString().Split(" ");
                string[]? cc = Encoding.UTF8.GetString(Convert.FromBase64String(m[1]))?.Split(":");
                if (cc?.Any() ?? false)
                {
                    using var httpclient = HttpClientProvider.CreateClient(HttpClientNames.Http);
                    var tokenresponse = await httpclient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                    {
                        ClientId = cc[0],
                        ClientSecret = cc[1],
                        Scope = "grpcapi-scope",
                        GrantType = "client_credentials",
                        Address = $"{ArsConfiguration.ArsIdentityClientConfiguration!.Authority}/connect/token"
                    });

                    if (tokenresponse.IsError)
                        return ArsOutput<LoginOutput>.Failed(1, tokenresponse.Error);

                    var datas = JsonConvert.DeserializeObject<LoginOutput>(tokenresponse.Json.ToString())!;
                    return ArsOutput<LoginOutput>.Success(datas);
                }
            }

            return ArsOutput<LoginOutput>.Failed(1, "参数错误");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ArsOutput<LoginOutput>> Password([FromBody] LoginInput input)
        {
            if (HttpContextAccessor.HttpContext?.Request?.Headers?.TryGetValue("Authorization", out StringValues value) ?? false)
            {
                var m = value.ToString().Split(" ");
                string[]? cc = Encoding.UTF8.GetString(Convert.FromBase64String(m[1]))?.Split(":");
                if (cc?.Any() ?? false)
                {
                    using var httpclient = HttpClientProvider.CreateClient(HttpClientNames.Http);
                    var tokenresponse = await httpclient.RequestPasswordTokenAsync(new PasswordTokenRequest
                    {
                        ClientId = cc[0],
                        ClientSecret = cc[1],
                        Scope = "grpcapi-scope",
                        GrantType = "password",
                        Address = $"{ArsConfiguration.ArsIdentityClientConfiguration!.Authority}/connect/token",
                        UserName = input.UserName,
                        Password = input.PassWord
                    });

                    if (tokenresponse.IsError)
                        return ArsOutput<LoginOutput>.Failed(1, tokenresponse.Error);

                    var datas = JsonConvert.DeserializeObject<LoginOutput>(tokenresponse.Json.ToString())!;

                    datas.user_name = "1";
                    return ArsOutput<LoginOutput>.Success(datas);
                }
            }

            return ArsOutput<LoginOutput>.Failed(1, "参数错误");
        }

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost(nameof(RefreshToken))]
        public async Task<ArsOutput<LoginOutput>> RefreshToken(RefreshTokenInput input)
        {
            if (HttpContextAccessor.HttpContext?.Request?.Headers?.TryGetValue("Authorization", out StringValues value) ?? false)
            {
                var m = value.ToString().Split(" ");
                string[]? cc = Encoding.UTF8.GetString(Convert.FromBase64String(m[1]))?.Split(":");
                if (cc?.Any() ?? false)
                {
                    using var httpclient = HttpClientProvider.CreateClient(HttpClientNames.Http);
                    var tokenresponse = await httpclient.RequestRefreshTokenAsync(new RefreshTokenRequest
                    {
                        ClientId = cc[0],
                        ClientSecret = cc[1],
                        RefreshToken = input.Refresh_token,
                        Scope = "grpcapi-scope",
                        GrantType = "refresh_token",
                        Address = "http://localhost:5105/connect/token"
                    });

                    if (tokenresponse.IsError)
                        return ArsOutput<LoginOutput>.Failed(1, tokenresponse.Error);

                    var datas = JsonConvert.DeserializeObject<LoginOutput>(tokenresponse.Json.ToString())!;
                    return ArsOutput<LoginOutput>.Success(datas);
                }
            }
            return ArsOutput<LoginOutput>.Failed(1, "参数错误");
        }
    }
}
