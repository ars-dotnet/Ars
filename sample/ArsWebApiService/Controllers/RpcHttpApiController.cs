using Ars.ArsWebApiService.HttpApi.Contract.IRpcContract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApiWithIdentityServer4.Controllers;
using static Org.BouncyCastle.Math.Primes;

namespace ArsWebApiService.Controllers
{
    [Authorize("default")]
    public class RpcHttpApiController : MyControllerBase
    {
        [HttpPost]
        public Task<GetOutput> PostFromForm([FromForm] GetInput input) 
        {
            return Task.FromResult(new GetOutput { Message = $"hello {input.Name} top {input.Top}" });
        }

        [HttpGet]
        public Task<GetOutput> GetFromQuery([FromQuery] GetInput input)
        {
            return Task.FromResult(new GetOutput { Message = $"hello {input.Name} top {input.Top}" });
        }

        [AllowAnonymous]
        [HttpGet("{where}/{when}")]
        public async Task<GetOutput> GetFromRoute([FromRoute] RouteInput route,[FromQuery] GetInput input)
        {
            await Task.Delay(1000 * 20);

            return new GetOutput
            {
                Message = $"hello {input.Name} top {input.Top} from {route.Where} when {route.When}"
            };
        }

        [HttpPost]
        public Task<GetOutput> PostFromBody([FromBody] GetInput input)
        {
            return Task.FromResult(new GetOutput { Message = $"hello {input.Name} top {input.Top}" });
        }
    }
}
