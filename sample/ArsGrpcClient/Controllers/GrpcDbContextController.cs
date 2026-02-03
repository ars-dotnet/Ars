using Ars.Common.Consul.GrpcHelper;
using Microsoft.AspNetCore.Mvc;
using Grpc.Core;
using Grpc.test;
using GrpcClients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace ArsGrpcClient.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("Api/GrpcClient/[controller]/[action]")]
    [Authorize("default")]
    public class GrpcDbContextController : Controller
    {
        private readonly IGrpcClientProviderByConsul _grpcClientProvider;

        private readonly IChannelManager _channelManager;

        public GrpcDbContextController(
            IGrpcClientProviderByConsul grpcClientProvider,
            IChannelManager channelManager)
        {
            _grpcClientProvider = grpcClientProvider;

            _channelManager = channelManager;
        }

        /// <summary>
        /// SayHelloUnaryCallAsync
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> UnaryCallAsync()
        {
            var client = await _grpcClientProvider.GetGrpcClient<TestRpc.TestRpcClient>("apigrpc1");

            var m = await client.SayHelloUnaryCallAsync(
                new HelloRequest() { Age = 1 }, 
                headers: new Metadata() { new Metadata.Entry("name", "ars") });

            return Json(m);
        }

        /// <summary>
        /// StreamFromClientCall
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> StreamFromClientCallAsync()
        {
            var client = await _grpcClientProvider.GetGrpcClient<TestRpc.TestRpcClient>("apigrpc1");

            var req = client.SayHelloStreamingFromClient();

            await _channelManager.WaitToReadAsync("grpc", req);

            _ = Task.Run(async () =>
            {
                int i = 1;

                while (i < 7)
                {
                    await _channelManager.WriteAsync("grpc", new HelloRequest { Age = i });

                    i++;
                    await Task.Delay(TimeSpan.FromSeconds(2));
                }
            });

            //_ = Task.Run(async () =>
            //{
            //    int i = 5;
            //    while (i > -1)
            //    {
            //        await _channelManager.WriteAsync("grpc", new HelloRequest { Age = i });

            //        i--;
            //        await Task.Delay(TimeSpan.FromSeconds(2));
            //    }
            //});

            return Ok();
        }

        /// <summary>
        /// StreamingFromServerCall
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> StreamingFromServerCallAsync()
        {
            CancellationTokenSource source = new CancellationTokenSource();

            var client = await _grpcClientProvider.GetGrpcClient<TestRpc.TestRpcClient>("apigrpc1");

            using var res = client.SayHelloStreamingFromServer(
                new HelloRequest() { Age = 0 }, 
                new CallOptions(cancellationToken: source.Token));

            //source.CancelAfter(TimeSpan.FromSeconds(10));

            IList<dynamic> datas = new List<dynamic>(6);

            await foreach (var msg in res.ResponseStream.ReadAllAsync())
            {
                datas.Add(msg?.Message ?? string.Empty);
            }

            return Json(string.Join(",", datas));
        }

        /// <summary>
        /// StreamBothWaysCall
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> StreamBothWaysCallAsync()
        {
            var client = await _grpcClientProvider.GetGrpcClient<TestRpc.TestRpcClient>("apigrpc1");

            using var req = client.SayHelloStreamingBothWays();

            for (int i = 1; i < 7; i++)
            {
                await req.RequestStream.WriteAsync(new HelloRequest { Age = i });
            }

            await req.RequestStream.CompleteAsync();

            IList<dynamic> datas = new List<dynamic>(6);

            await foreach (var res in req.ResponseStream.ReadAllAsync())
            {
                datas.Add(res.Message);
            }

            return Json(string.Join(",", datas));
        }
    }
}
