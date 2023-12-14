using Ars.Common.Consul;
using Ars.Common.Consul.HttpClientHelper;
using Ars.Common.Tool.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrpcClient.Controllers
{
    [ApiController]
    [Route("Api/GrpcClient/[controller]/[action]")]
    [Authorize("default")]
    public class ArsWebApiController : Controller
    {
        private readonly IHttpClientProviderByConsul _httpClientProvider;
        private readonly IHttpSender _httpSender;
        public ArsWebApiController(IHttpClientProviderByConsul httpClientProvider, IHttpSender httpSender)
        {
            _httpClientProvider = httpClientProvider;
            _httpSender = httpSender;
        }

        [HttpGet]
        public async Task<IActionResult> RpcArsWebApi() 
        {
            using var httpclient = await _httpClientProvider.GetHttpClientAsync<HttpClient>("arswebapiservice");
            var data = await _httpSender.GetAsync(httpclient, "/Api/ArsWebApi/DbContext/Query/Query");
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> RpcArsWebApiBroker()
        {
            using var httpclient = await _httpClientProvider.GetHttpClientAsync<HttpClient>("arswebapiservice",HttpClientNames.RetryHttp);
            var data = await _httpSender.GetAsync(httpclient, "/Api/ArsWebApi/Ocelot/TimeoutRejectedException");
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> RpcArsWebThrow()
        {
            using var httpclient = await _httpClientProvider.GetHttpClientAsync<HttpClient>("arswebapiservice", HttpClientNames.RetryHttp);
            var data = await _httpSender.GetAsync(httpclient, "/Api/ArsWebApi/Ocelot/HttpRequestException");
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> HttpClientMultiRead()
        {
            using var httpclient = await _httpClientProvider.GetHttpClientAsync<HttpClient>("arswebapiservice", "test");
            var data = await _httpSender.GetAsync<object>(httpclient, "/Api/ArsWebApi/DbContext/Query/Query");
            return Ok(data);
        }
    }
}
