using System.Threading.Channels;

namespace GrpcClients
{
    public interface IChannelProvider
    {
        public Channel<T> GetOrAddChannel<T>(string name);
    }
}
