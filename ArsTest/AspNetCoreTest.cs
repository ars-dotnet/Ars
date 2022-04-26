using Ars.Commom.Tool;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ArsTest
{
    public class Person
    {
        public string Name { get; set; }

        public int Age { get; set; }
    }

    public class AspNetCoreBase
    {
        public IServiceCollection services { get; private set; }
        public AspNetCoreBase()
        {
            IHost host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddJsonFile("config.json", true, false);
                })
                .ConfigureServices((builder, service) =>
                {
                    services = service;
                })
                .Build();
        }
    }

    public class TestIncrement
    {
        public int i;
        public int j;

        public async Task<int> Set() 
        {
            await Task.Yield();
            var m = Interlocked.Increment(ref i);
            return m;
        }
    }

    public abstract class A 
    {
        public abstract void Get();
    }

    public class AA : A 
    {
        public override void Get()
        {

        }
    }

    public class AAA : AA 
    {
        public override void Get()
        {

        }
    }

    public class AspNetCoreTest : AspNetCoreBase
    {
        [Fact]
        public void TestOverride() 
        {
            A a = new AAA();
            a.Get();
            a = new AA();
            a.Get();
        }

        [Fact]
        public async Task TestIncrement1() 
        {
            IList<Task<int>> tasks = new List<Task<int>>();
            TestIncrement obj = new TestIncrement();
            var b = Task.Run(() =>
            {
                for (int i = 0; i < 500; i++)
                {
                    tasks.Add(obj.Set());
                }
            });
            var a = Task.Run(() =>
            {
                for (int i = 0; i < 500; i++)
                {
                    tasks.Add(obj.Set());
                }
            });

            await Task.WhenAll(a, b);
            var m = await Task.WhenAll(tasks);
            m = m.OrderBy(r => r).ToArray();
            var x = string.Join(",", m);
        }

        [Fact]
        public void TestA() 
        {
            int a = 0;
            TestB(a);
            Testc(ref a);
        }

        private void TestB(int a)
        {
            a = 2;
        }
        private void Testc(ref int a) 
        {
            a = 3;
        }

        [Fact]
        public void TestDispose() 
        {
            DisposeAction dispose = new DisposeAction(() =>
            {
                return Task.FromResult(0);
            });

            dispose.Dispose();
            dispose.Dispose();
        }

        [Fact]
        public void CreateHost() 
        {
            int[] array = { 1, 2, 3, 4, 5, 6 };
            Array.ForEach(array, r => 
            {
                r++;
            });
        }

        [Fact]
        public async Task TestIdentityServer() 
        {
            using HttpClient client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:7207");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }
            var tokenresponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                ClientId = "default-Key",
                ClientSecret = "default-Secret",
                Scope = "defaultApi-scope",
                UserName = "test",
                Password = "test",
                Address = "http://localhost:7207/connect/token"
            });
            if(tokenresponse.IsError)
            {
                Console.WriteLine(tokenresponse.Error);
                return;
            }

            var token = tokenresponse.AccessToken;

            //http://localhost:5196/WeatherForecast
            // call api
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(token);

            var response = await apiClient.GetAsync("http://localhost:5196/WeatherForecast");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }
        }

        [Fact]
        public void TestOption() 
        {
            var serviceProvider = services.BuildServiceProvider();
            IConfiguration config = serviceProvider.GetRequiredService<IConfiguration>();

            services.Configure<Person>(config.GetSection(nameof(Person)));
            var a = services.BuildServiceProvider().GetRequiredService<IOptions<Person>>().Value;

            services.AddSingleton<IOptions<Person>>(new OptionsWrapper<Person>(new Person { Age = 12333 }));
            var b = services.BuildServiceProvider().GetRequiredService<IOptions<Person>>().Value;
        }

        [Fact]
        public void TestPath() 
        {
            var a = Directory.GetCurrentDirectory();//这个代表当前应用程序的根目录
            var b = AppDomain.CurrentDomain.BaseDirectory; //web程序中，这个代表当前应用程序运行的根目录
        }
    }
}
