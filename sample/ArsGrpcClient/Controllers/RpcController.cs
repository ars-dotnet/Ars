using Ars.ArsWebApiGrpcService.HttpApi;
using Ars.ArsWebApiService.HttpApi;
using Ars.ArsWebApiService.HttpApi.Contract.IRpcContract;
using Ars.Common.Core.IDependency;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArsGrpcClient.Controllers
{
    [Route("Api/Rpc/[controller]/[action]")]
    [ApiController]
    [Authorize("default")]
    public class RpcController : Controller
    {
        /// <summary>
        /// rpc测试1
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> WeatherForecastHttpApiAction(
            [FromServices] IWeatherForecastHttpApi WeatherForecastHttpApi)
        {
            var datas = await WeatherForecastHttpApi.Get();

            return Json(datas);
        }

        /// <summary>
        /// rpc测试2
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> DbHttpApiAction([FromServices] IDbHttpApi DbHttpApi)
        {
            var datas = await DbHttpApi.Query();

            await DbHttpApi.ModifyWithDefaultTransaction();

            return Ok(datas);
        }

        /// <summary>
        /// rpc测试3
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> RpcHttpApiAction([FromServices] IRpcHttpApi RpcHttpApi) 
        {
            GetInput input = new GetInput { Name = "ars",Top = "175cm" };

            var data3 = await RpcHttpApi.GetFromRoute(
                new RouteInput { Where = "China", When = "Yesterday" },
                input);

            var data1 = await RpcHttpApi.PostFromForm(input);

            var data2 = await RpcHttpApi.GetFromQuery(input);

            var data4 = await RpcHttpApi.PostFromBody(input);

            return Ok((data1,data2,data3,data4));
        }
    }
}
