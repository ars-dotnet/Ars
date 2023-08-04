using Grpc.Core;
using GrpcGreeter.greet;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Channels;
using Channel = System.Threading.Channels.Channel;

namespace GrpcService.Services
{
    [Authorize]
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }

        public override async Task StreamingFromServer(StreamingRequest request,
            IServerStreamWriter<StreamingResponse> responseStream,
            ServerCallContext context)
        {
            decimal value = request.Value;
            while (!context.CancellationToken.IsCancellationRequested)
            {
                value += 0.1m;
                await responseStream.WriteAsync(new StreamingResponse { Value = value });
                await Task.Delay(1000, context.CancellationToken);
            }
        }

        public override async Task<StreamingResponse> StreamingFromClient(
            IAsyncStreamReader<StreamingRequest> requestStream,
            ServerCallContext context)
        {
            var channel = Channel.CreateUnbounded<StreamingRequest>();
            _ = Task.Run(async () =>
            {
                await foreach (var data in channel.Reader.ReadAllAsync())
                {
                    Console.WriteLine(data.Value);
                }
            });
            await foreach (var req in requestStream.ReadAllAsync())
            {
                await channel.Writer.WriteAsync(req);
            }

            return new StreamingResponse();
        }

        public override async Task streamingBothWays(
            IAsyncStreamReader<StreamingRequest> requestStream,
            IServerStreamWriter<StreamingResponse> responseStream,
            ServerCallContext context)
        {
            var channel = Channel.CreateUnbounded<StreamingResponse>();
            var consumerTask = Task.Run(async () =>
            {
                await foreach (var message in channel.Reader.ReadAllAsync()) 
                {
                    await responseStream.WriteAsync(message);
                }
            });

            var dataChunks = await requestStream.ReadAllAsync().ToListAsync();
            await Task.WhenAll(dataChunks.Select(r =>
            {
                return Task.Run(async () =>
                {
                    await channel.Writer.WriteAsync(new StreamingResponse { Value = r.Value });
                });
            }));

            channel.Writer.Complete();
            await consumerTask;
        }
    }
}