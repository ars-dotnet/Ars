using Ars.Common.Consul.GrpcHelper;
using Microsoft.AspNetCore.Mvc;
using GrpcGreeter.greet;
using Grpc.Core;
using System.Threading.Channels;
using Grpc.Net.Client;

namespace GrpcClients.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class GrpcApiController : Controller
    {
        private readonly IGrpcClientProvider _grpcClientProvider;
        private readonly IChannelManager _channelManager;
        public GrpcApiController(IGrpcClientProvider grpcClientProvider,
            IChannelManager channelManager)
        {
            _grpcClientProvider = grpcClientProvider;
            _channelManager = channelManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UnaryCall()
        {
            var client = await _grpcClientProvider.GetGrpcClient<Greeter.GreeterClient>("apigrpc");
            var m = await client.SayHelloAsync(new HelloRequest() { Name = "Bill" });
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
            var client = await _grpcClientProvider.GetGrpcClient<Greeter.GreeterClient>("apigrpc");
            using var req = client.StreamingFromClient();

            var channel = Channel.CreateBounded<StreamingRequest>(15);
            _ = Task.Run(async () =>
            {
                while (await channel.Reader.WaitToReadAsync())
                {
                    while (channel.Reader.TryRead(out var msg))
                    {
                        await req.RequestStream.WriteAsync(msg);
                    }
                }
            });

            _ = Task.Run(async () =>
            {
                int i = 0;
                while (true) 
                {
                    //读取plc的值，丢到channel里面
                    await Task.Delay(TimeSpan.FromSeconds(2));
                    await _channelManager.WriteAsync("grpc", new StreamingRequest { Value = i });

                    //await channel.Writer.WriteAsync(new StreamingRequest() { Value = i });

                    i++;
                }
            });

            _ = Task.Run(async () =>
            {
                int i = 0;
                while (true)
                {
                    //读取plc的值，丢到channel里面
                    await Task.Delay(TimeSpan.FromSeconds(2));
                    await _channelManager.WriteAsync("grpc", new StreamingRequest { Value = i });
                    //await channel.Writer.WriteAsync(new StreamingRequest() { Value = i });

                    i++;
                }
            });

            //await req.RequestStream.CompleteAsync();
            await req.ResponseAsync;
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
