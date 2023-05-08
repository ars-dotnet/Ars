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
        private readonly IGrpcClientProvider _grpcClientProvider;
        private readonly IChannelManager _channelManager;
        private readonly IEntrySegmentContextAccessor _entrySegmentContext;
        public GrpcApiController(IGrpcClientProvider grpcClientProvider,
            IChannelManager channelManager,
            IEntrySegmentContextAccessor entrySegmentContext)
        {
            _grpcClientProvider = grpcClientProvider;
            _channelManager = channelManager;
            _entrySegmentContext = entrySegmentContext;
        }

        [HttpGet]
        public IActionResult Index(string a)
        {
            _entrySegmentContext.Context.Span.AddLog(LogEvent.Message("index 1"));
            return Ok(a + ":great");
        }

        [HttpPost]
        public IActionResult PostTest([FromBody]IDictionary<string,string> a) 
        {
            return Ok(string.Join(",", a));
        }

        [HttpPost]
        public async Task<IActionResult> UnaryCallRestful()
        {
            var client = await _grpcClientProvider.GetGrpcClient<Greeter.GreeterClient>("apigrpc");
            var m = await client.SayHelloAsync(new HelloRequest() { Name = "ars" });
            return Json(m);
        }

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
                    //读取plc的值，丢到channel里面
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
                    //读取plc的值，丢到channel里面
                    await _channelManager.WriteAsync("grpc", new StreamingRequest { Value = i });

                    i++;
                    await Task.Delay(TimeSpan.FromSeconds(2));
                }
            });

            //var client = await _grpcClientProvider.GetGrpcClient<Greeter.GreeterClient>("apigrpc1");
            //var req = client.StreamingFromClient();
            //await _channelManager.WaitToReadAsync("grpc", req);
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
