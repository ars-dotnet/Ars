using Ars.Common.Consul;
using Microsoft.AspNetCore.Mvc;

namespace GrpcClient.Controllers
{
    public class ArsWebApiController : Controller
    {
        private readonly ConsulHelper _consulHelper; 
        public ArsWebApiController(ConsulHelper consulHelper)
        {
            _consulHelper = consulHelper;
        }

        public Task RpcArsWebApi() 
        {
            return Task.CompletedTask;
        }
    }
}
