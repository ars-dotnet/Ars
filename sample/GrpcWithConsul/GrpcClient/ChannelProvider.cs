using System.Collections.Concurrent;
using System.Threading.Channels;

namespace GrpcClients
{
    public class ChannelProvider : IChannelProvider
    {
        private readonly ConcurrentDictionary<string,object> channels;
        public ChannelProvider()
        {
            channels = new ConcurrentDictionary<string, object>();
        }

        public Channel<T> GetOrAddChannel<T>(string name)
        {
            var value = channels.GetOrAdd(name, Channel.CreateUnbounded<T>());
            return (value as Channel<T>)!;
        }
    }
}
