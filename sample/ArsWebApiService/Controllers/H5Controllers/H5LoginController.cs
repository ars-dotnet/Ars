using Ars.Common.Core.AspNetCore.OutputDtos;
using Ars.Common.Tool.Tools;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using MyApiWithIdentityServer4.Controllers;
using MyApiWithIdentityServer4.Dtos;
using Newtonsoft.Json;
using System.Text;

namespace ArsWebApiService.Controllers.H5Controllers
{
    public class H5LoginController : MyControllerBase
    {
        [HttpPost]
        public async Task<ArsOutput<LoginOutput>> Login([FromForm] LoginInput input)
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

                    datas.user_name = input.UserName;
                    return ArsOutput<LoginOutput>.Success(datas);
                }
            }

            return ArsOutput<LoginOutput>.Failed(1, "参数错误");
        }
    }
}
