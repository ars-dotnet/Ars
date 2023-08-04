using Ars.Common.Consul.GrpcHelper;
using Microsoft.AspNetCore.Mvc;
using GrpcGreeter.greet;
using Grpc.Net.Client;
using Grpc.Core;
using Channel = System.Threading.Channels.Channel;
using SkyApm.Tracing;
using SkyApm.Tracing.Segments;

namespace GrpcClients.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class GrpcApiController : Controller
    {
        private readonly IGrpcClientProviderByConsul _grpcClientProvider;
        private readonly IChannelManager _channelManager;
        public GrpcApiController(IGrpcClientProviderByConsul grpcClientProvider,
            IChannelManager channelManager)
        {
            _grpcClientProvider = grpcClientProvider;
            _channelManager = channelManager;
        }

        /// <summary>
        /// 调用纯grpc服务端
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UnaryCallGrpc()
        {
            var client = await _grpcClientProvider.GetGrpcClient<Greeter.GreeterClient>("apigrpc");
            var m = await client.SayHelloAsync(new HelloRequest() { Name = "ars" });
            return Json(m);
        }

        /// <summary>
        /// 调用支持restfulapi的grpc服务端
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UnaryCallGrpc1()
        {
            var client = await _grpcClientProvider.GetGrpcClient<Greeter.GreeterClient>("apigrpc1");
            var m = await client.SayHelloAsync(new HelloRequest() { Name = "ars" }, headers:new Metadata() { new Metadata.Entry("name","ars")});
            return Json(m);
        }

        [HttpPost]
        public async Task<IActionResult> StreamingFromServerCall()
        {
            CancellationTokenSource source = new CancellationTokenSource();
            var client = await _grpcClientProvider.GetGrpcClient<Greeter.GreeterClient>("apigrpc");
            using var res = client.StreamingFromServer(new StreamingRequest() { Value = 10.1m }, new CallOptions(cancellationToken: source.Token));

            source.CancelAfter(TimeSpan.FromSeconds(10));

            IList<decimal> datas = new List<decimal>();
            await foreach (var msg in res.ResponseStream.ReadAllAsync())
            {
                datas.Add(msg.Value);
            }

            return Json(string.Join(",", datas));
        }

        [HttpPost]
        public async Task<IActionResult> StreamFromClientCall()
        {
            _ = Task.Run(async () =>
            {
                int i = 0;
                while (true) 
                {
                    await _channelManager.WriteAsync("grpc", new StreamingRequest { Value = i });

                    i++;
                    await Task.Delay(TimeSpan.FromSeconds(2));
                }
            });

            _ = Task.Run(async () =>
            {
                int i = 0;
                while (true)
                {
                    await _channelManager.WriteAsync("grpc", new StreamingRequest { Value = i });

                    i++;
                    await Task.Delay(TimeSpan.FromSeconds(2));
                }
            });

            var client = await _grpcClientProvider.GetGrpcClient<Greeter.GreeterClient>("apigrpc1");
            var req = client.StreamingFromClient();
            await _channelManager.WaitToReadAsync("grpc", req);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> StreamFromClientCallConsume() 
        {
            var client = await _grpcClientProvider.GetGrpcClient<Greeter.GreeterClient>("apigrpc1");
            var req = client.StreamingFromClient();
            await _channelManager.WaitToReadAsync("grpc", req);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> StreamBothWaysCall()
        {
            var client = await _grpcClientProvider.GetGrpcClient<Greeter.GreeterClient>("apigrpc");
            using var req = client.streamingBothWays();

            for (int i = 0; i < 10; i++)
            {
                await req.RequestStream.WriteAsync(new StreamingRequest { Value = i });
            }
            await req.RequestStream.CompleteAsync();

            IList<decimal> datas = new List<decimal>(0);
            await foreach (var res in req.ResponseStream.ReadAllAsync())
            {
                datas.Add(res.Value);
            }

            return Json(string.Join(",", datas));
        }

    }
}
