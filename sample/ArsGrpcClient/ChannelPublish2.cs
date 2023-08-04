using GrpcGreeter.greet;

namespace GrpcClients
{
    public class ChannelPublish2 : BackgroundService
    {
        private readonly IChannelManager _channelManager;
        public ChannelPublish2(IChannelManager channelManager)
        {
            _channelManager = channelManager;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _ = Task.Run(async () =>
            {
                int i = 0;
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
                int i = 0;
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
