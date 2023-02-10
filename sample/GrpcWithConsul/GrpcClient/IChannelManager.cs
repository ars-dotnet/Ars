namespace GrpcClients
{
    public interface IChannelManager
    {
        public ValueTask WriteAsync<T>(string name,T t);

        public IAsyncEnumerable<T> ReadAllAsync<T>(string name);
    }
}
