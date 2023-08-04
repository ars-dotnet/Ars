using Ars.Common.Consul;
using Ars.Common.Consul.HttpClientHelper;
using Microsoft.AspNetCore.Mvc;

namespace GrpcClient.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ArsWebApiController : Controller
    {
        private readonly IHttpClientProviderByConsul _httpClientProvider;
        private readonly IHttpSender _httpSender;
        public ArsWebApiController(IHttpClientProviderByConsul httpClientProvider, IHttpSender httpSender)
        {
            _httpClientProvider = httpClientProvider;
            _httpSender = httpSender;
        }

        [HttpPost]
        public async Task<IActionResult> RpcArsWebApi() 
        {
            using var httpclient = await _httpClientProvider.GetHttpClientAsync<HttpClient>("arswebapiservice");
            var data = await _httpSender.GetAsync(httpclient, "/Api/DbContext/Query/Query");
            return Ok(data);
        }
    }
}
