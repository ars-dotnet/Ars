using Grpc.Core;
using Org.BouncyCastle.Ocsp;
using System.Threading.Channels;

namespace GrpcClients
{
    public class ChannelManager : IChannelManager
    {
        private readonly IChannelProvider _provider;
        public ChannelManager(IChannelProvider provider)
        {
            _provider = provider;
        }

        public ValueTask WriteAsync<T>(string name,T t)
        {
            return _provider.GetOrAddChannel<T>(name).Writer.WriteAsync(t);
        }

        public IAsyncEnumerable<T> ReadAllAsync<T>(string name)
        {
            return _provider.GetOrAddChannel<T>(name).Reader.ReadAllAsync();
        }

        public Task WaitToReadAsync<TRequest, TResponse>(string name, AsyncClientStreamingCall<TRequest, TResponse> streamingCall)
        {
            var channel = _provider.GetOrAddChannel<TRequest>(name);
            _ = Task.Run(async () =>
            {
                while (await channel.Reader.WaitToReadAsync())
                {
                    while (channel.Reader.TryRead(out var msg))
                    {
                        await streamingCall.RequestStream.WriteAsync(msg);
                    }
                }
            });

            return Task.CompletedTask;
        }
    }
}
