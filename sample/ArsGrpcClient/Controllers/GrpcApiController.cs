﻿using Ars.Common.Consul.GrpcHelper;
using Microsoft.AspNetCore.Mvc;
using GrpcGreeter.greet;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Ars.Common.Consul.HttpClientHelper;

namespace GrpcClients.Controllers
{
    [ApiController]
    [Route("Api/GrpcClient/[controller]/[action]")]
    [Authorize("default")]
    public class GrpcApiController : Controller
    {
        private readonly IGrpcClientProviderByConsul _grpcClientProvider;
        private readonly IHttpClientProviderByConsul _httpClientProvider;
        private readonly IHttpSender _httpSender;
        private readonly IChannelManager _channelManager;
        public GrpcApiController(
            IGrpcClientProviderByConsul grpcClientProvider,
            IHttpClientProviderByConsul httpClientProviderByConsul,
            IHttpSender httpSender,
            IChannelManager channelManager)
        {
            _grpcClientProvider = grpcClientProvider;
            _httpClientProvider = httpClientProviderByConsul;
            _httpSender = httpSender;
            _channelManager = channelManager;
        }

        /// <summary>
        /// 调用支持restfulapi的grpc服务端
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UnaryCallGrpcWebApi()
        {
            try
            {
                using var httpclient = await _httpClientProvider.GetGrpcHttpClientAsync<HttpClient>("apigrpcwebapiservice");
                var data = await _httpSender.GetAsync(httpclient, "/Api/ArsGrpcWebApi/WeatherForecast/Get");

                return Ok(data);
            }
            catch (Exception e) 
            {

            }
           
            return BadRequest();
        }

        /// <summary>
        /// 调用支持restfulapi的grpc服务端
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UnaryCallGrpc()
        {
            var client = await _grpcClientProvider.GetGrpcClient<Greeter.GreeterClient>("apigrpcwebapiservice");
            var m = await client.SayHelloAsync(new HelloRequest() { Name = "ars" });
            return Json(m);
        }

        /// <summary>
        /// 调用纯grpc服务端
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
            var client = await _grpcClientProvider.GetGrpcClient<Greeter.GreeterClient>("apigrpcwebapiservice");
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
            var client = await _grpcClientProvider.GetGrpcClient<Greeter.GreeterClient>("apigrpcwebapiservice");
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
