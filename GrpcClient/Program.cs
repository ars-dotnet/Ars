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
            var channel = GrpcChannel.ForAddress("https://localhost:7002");
            var client = new Greeter.GreeterClient(channel);
            var res = await client.SayHelloAsync(new HelloRequest
            {
                Name = "test"
            });

            Console.WriteLine(res.Message);
            Console.Read();
        }
    }
}
