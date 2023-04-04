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
    }
}
