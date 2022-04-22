using Grpc.Core;
using Grpc.Net.Client;
using GrpcGreeter.greet;
using System;
using System.Threading.Tasks;

namespace GrpcClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var client = Get<Greeter.GreeterClient>();
            var res = await client.SayHelloAsync(new HelloRequest
            {
                Name = "test"
            });

            var a = await client.TestAsync(new TestInput());

            Console.WriteLine(res.Message);
            Console.Read();
        }

        static T Get<T>()
            where T : ClientBase<T>
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7902");
            return (T)Activator.CreateInstance(typeof(T), channel);
        }
    }
}
