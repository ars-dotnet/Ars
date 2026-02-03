using Ars.Common.Core.Uow.Attributes;
using Ars.Common.EFCore.Repository;
using ArsWebApiService.Model;
using Grpc.Core;
using Grpc.test;
using Org.BouncyCastle.Ocsp;
using Channel = System.Threading.Channels.Channel;

namespace ArsGrpcService.Services
{
    public class GrpcDbContextService : TestRpc.TestRpcBase
    {
        private readonly IRepository<StudentNew,Guid> _repo;
        
        public GrpcDbContextService(IRepository<StudentNew, Guid> repo)
        {
            _repo = repo;
        }

        [UnitOfWork]
        public override async Task<HelloReply> SayHelloUnaryCall(
            HelloRequest request, 
            ServerCallContext context)
        {
            var data = await _repo.FirstOrDefaultAsync(r => r.Age == request.Age);

            return new HelloReply() { Message = string.Concat(nameof(SayHelloUnaryCall),":",data?.Name, "+", data?.Age) };
        }

        [UnitOfWork(IsDisabled = true)]
        public override async Task<HelloReply> SayHelloStreamingFromClient(
            IAsyncStreamReader<HelloRequest> requestStream,
            ServerCallContext context)
        {
            var channel = Channel.CreateUnbounded<HelloRequest>();

            _ = Task.Run(async () =>
            {
                await foreach (var req in channel.Reader.ReadAllAsync())
                {
                    var data = await _repo.FirstOrDefaultAsync(r => r.Age == req.Age);

                    _repo.Dispose();

                    Console.WriteLine(string.Concat(nameof(SayHelloStreamingFromClient), ":", data?.Name, "+", data?.Age));
                }
            });

            await foreach (var req in requestStream.ReadAllAsync())
            {
                await channel.Writer.WriteAsync(req);
            }

            return new HelloReply();
        }

        public override async Task SayHelloStreamingFromServer(
            HelloRequest request, 
            IServerStreamWriter<HelloReply> responseStream, 
            ServerCallContext context)
        {
            while (!context.CancellationToken.IsCancellationRequested) 
            {
                int age = request.Age;

                for (var i = 0; i < 6; i++) 
                {
                    age += 1;

                    var data = await _repo.FirstOrDefaultAsync(r => r.Age == age);

                    await responseStream.WriteAsync(new HelloReply
                    { 
                        Message = string.Concat(nameof(SayHelloUnaryCall), ":", data?.Name, "+", data?.Age) 
                    });

                    await Task.Delay(1000, context.CancellationToken);
                }

                break;
            }
        }

        [UnitOfWork(IsDisabled = true)]
        public override async Task SayHelloStreamingBothWays(
            IAsyncStreamReader<HelloRequest> requestStream, 
            IServerStreamWriter<HelloReply> responseStream, 
            ServerCallContext context)
        {
            var channel = Channel.CreateUnbounded<HelloReply>();

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
                    var data = await _repo.FirstOrDefaultAsync(t => t.Age == r.Age);

                    _repo.Dispose();

                    await channel.Writer.WriteAsync(new HelloReply 
                    {
                        Message = string.Concat(
                            nameof(SayHelloStreamingBothWays),":",data?.Name, "+", data?.Age) 
                    });
                });
            }));

            channel.Writer.Complete();

            await consumerTask;
        }
    }
}
