using Ars.Common.Consul.GrpcHelper;
using GrpcGreeter.greet;

namespace GrpcClients
{
    public class ChannelPublish : BackgroundService
    {
        private readonly IChannelManager _channelManager;
        private readonly IGrpcClientProvider _grpcClientProvider;
        public ChannelPublish(IChannelManager channelManager, IGrpcClientProvider grpcClientProvider)
        {
            _channelManager = channelManager;
            _grpcClientProvider = grpcClientProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _ = Task.Run(async () =>
            {
                int i = 999;
                while (true)
                {
                    //读取plc的值，丢到channel里面
                    await Task.Delay(TimeSpan.FromSeconds(2));
                    await _channelManager.WriteAsync("grpc", new StreamingRequest { Value = i });
                    i++;
                }
            });

            _ = Task.Run(async () =>
            {
                int i = 999;
                while (true)
                {
                    //读取plc的值，丢到channel里面
                    await Task.Delay(TimeSpan.FromSeconds(2));
                    await _channelManager.WriteAsync("grpc", new StreamingRequest { Value = i });
                    i++;
                }
            });

            return Task.CompletedTask;
        }
    }
}
