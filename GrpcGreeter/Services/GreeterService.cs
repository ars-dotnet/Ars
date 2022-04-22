using Grpc.Core;
using GrpcGreeter.greet;

namespace GrpcGreeter.Services
{
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
                Message = "Hello: " + request.Name,
                Value = 12121
            });
        }

        public override Task<TestOutput> Test(TestInput request, ServerCallContext context)
        {
            return Task.FromResult(new TestOutput { Code = 200 });
        }

        public override Task<RpcCheckOutput> Check(RpcCheckInput request, ServerCallContext context)
        {
            return Task.FromResult(new RpcCheckOutput { Res = "ok" });
        }
    }
}