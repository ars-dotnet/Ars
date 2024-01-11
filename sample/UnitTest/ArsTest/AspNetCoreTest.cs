using Ars.Commom.Tool;
using Ars.Common.Core.IDependency;
using Ars.Common.Tool.Extension;
using ArsTest.AppConfigs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Xunit;

namespace ArsTest
{


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
                    service.AddLogging();
                    service.AddHttpClient("ars");

                    service.AddScoped<IFater,Child>();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddDebug();
                })
                .Build();
        }
    }

    public interface IFater { }

    public abstract class BaseFater : IFater
    {
        [Autowired]
        public IHttpClientFactory Factory { get; set; }

        public BaseFater()
        {

        }
    }

    public class Child : BaseFater 
    {

    }

    public class AspNetCoreTest : AspNetCoreBase
    {
        [Fact]
        public void TestAutoWired() 
        {
            var a = services.BuildServiceProvider().GetRequiredService<IFater>();
        }

        [Fact]
        public void TestOverride()
        {
            var factory = services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
            var logger = factory.CreateLogger<AspNetCoreTest>();
            A a = new AAA();
            a.Get();
            a = new AA();
            a.Get();

            logger.LogDebug("my-test");
        }

        class TestIncrement
        {
            /// <summary>
            /// volatile关键字
            /// </summary>
            public int i = 0; 
            public int j;

            public async Task<int> Set()
            {
                await Task.Yield();

                Interlocked.Increment(ref i);

                return i;
            }
        }

        [InlineData(3)]
        [Theory]
        public async Task TestIncrement1(int ccc)
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

            Assert.True(m.Count() == 1000);
        }

        /// <summary>
        /// test ref keyword
        /// </summary>
        [Fact]
        public void TestA()
        {
            int a = 0;
            TestB(a);
            Testc(ref a);

            void TestB(int a)
            {
                a = 2;
            }
            void Testc(ref int a)
            {
                a = 3;
            }
        }


        [Fact]
        public void TestDispose()
        {
            DisposeAction dispose = new DisposeAction(() =>
            {
                Task.FromResult(0);
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

        //[Fact]
        //public async Task TestIdentityServer()
        //{
        //    using HttpClient client = new HttpClient();
        //    var disco = await client.GetDiscoveryDocumentAsync("http://localhost:7207");
        //    if (disco.IsError)
        //    {
        //        Console.WriteLine(disco.Error);
        //        return;
        //    }
        //    var tokenresponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
        //    {
        //        ClientId = "default-Key",
        //        ClientSecret = "default-Secret",
        //        Scope = "defaultApi-scope",
        //        UserName = "test",
        //        Password = "test",
        //        Address = "http://localhost:7207/connect/token"
        //    });
        //    if (tokenresponse.IsError)
        //    {
        //        Console.WriteLine(tokenresponse.Error);
        //        return;
        //    }

        //    var token = tokenresponse.AccessToken;

        //    //http://localhost:5196/WeatherForecast
        //    // call api
        //    var apiClient = new HttpClient();
        //    apiClient.SetBearerToken(token);

        //    var response = await apiClient.GetAsync("http://localhost:5196/WeatherForecast");
        //    if (!response.IsSuccessStatusCode)
        //    {
        //        Console.WriteLine(response.StatusCode);
        //    }
        //    else
        //    {
        //        var content = await response.Content.ReadAsStringAsync();
        //        Console.WriteLine(JArray.Parse(content));
        //    }
        //}

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

        [Fact]
        public void TestAA()
        {
            services.AddSingleton<IM, M>();

            var serviceProvider = services.BuildServiceProvider();
            var a = serviceProvider.GetRequiredService<IM>();
            a.Age = 1234;

            var v = serviceProvider.GetRequiredService<IM>();
        }

        [Fact]
        public void TestBB()
        {
            services.AddTransient<IM, M>();
            services.AddTransient<IM, MM>();

            services.AddTransient<ICheck, Check>();
            var serviceProvider = services.BuildServiceProvider();
            var a = serviceProvider.GetRequiredService<IM>();
        }

        [Fact]
        public void TestChunk()
        {
            IEnumerable<int> datas = new List<int>
            {
                1,2,3,4,5,6,7
            };

            var m = datas.Chunk(3);
        }

        [Fact]
        public void TestIOptions()
        {
            var a = services.BuildServiceProvider().GetRequiredService<IOptions<Configs>>();
            Assert.True("Bill".Equals(a.Value.Name));
        }

        [Fact]
        public void Test123()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddTransient<ICheck, Check>();
            services.AddTransient<ICheck, Check>();

            using var scope = services.BuildServiceProvider().CreateScope();
            var m = scope.ServiceProvider.GetServices<ICheck>();

            var a = m.First().GetHashCode();
            var b = m.Last().GetHashCode();
        }

        [Fact]
        public void Test99()
        {
            Goods goods = new Goods
            {
                Age = 123
            };

            //不能赋值
            //goods.Age = 222;
        }

        [Fact]
        public async Task TestWebServicePost()
        {
            try
            {
                var provider = services.BuildServiceProvider().CreateScope().ServiceProvider;
                var httpclientfac = provider.GetRequiredService<IHttpClientFactory>();
                using var httpclient = httpclientfac.CreateClient("ars");

                StudentModel studentModel = new StudentModel() { Id = 2, No = "223" };
                string xml = studentModel.XMLSerialize();

                TestXml testXml = new TestXml
                {
                    Name = "Tom",
                    Country = "China",
                    Item = new List<Item> 
                    {
                        new Item
                        {
                            TagCode = "top",
                            TagValue = "175",
                            TimeStamp = "1212212"
                        },
                        new Item
                        {
                            TagCode = "sex",
                            TagValue = "男",
                            TimeStamp = "1212212"
                        },
                        new Item
                        {
                            TagCode = "like",
                            TagValue = "music",
                            TimeStamp = "1212212"
                        },
                    }
                };
                string a = testXml.XMLSerialize();
                var data = a.DeXmlSerialize<TestXml>();


                using var res = await httpclient.GetAsync($"http://127.0.0.1:5196/WebServices.asmx/Publish?xml={xml}");

                res.EnsureSuccessStatusCode();

                var str = await res.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {

            }
        }

        [Fact]
        public async Task TestWebService()
        {
            var provider = services.BuildServiceProvider().CreateScope().ServiceProvider;
            var httpclientfac = provider.GetRequiredService<IHttpClientFactory>();
            using var httpclient = httpclientfac.CreateClient("ars");

            using var res = await httpclient.GetAsync("http://127.0.0.1:5196/WebServices.asmx/EchoWithGet?s=aabb");
            res.EnsureSuccessStatusCode();

            var str = await res.Content.ReadAsStringAsync();

            var data = str.DeXmlSerialize<StudentModel>();
            Assert.True(data!.Id == 1);
        }

        [Fact]
        public Task TestXmlPath()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppConfigs/App.Config");
            var data = path.DeXmlPathSerialize<Configuration>();

            return Task.CompletedTask;
        }

        [Fact]
        public void TestStr()
        {
            string barcode = @"
TT=90ms OTL=10mm CC=1 FC=266, MC=1
N6CCL36
QR CODE
SZ=21x21, RES=8.00x8.00Pixel, CTV=0%, UECV=38%
";

            if (barcode.Contains("N6CCL"))
            {
                barcode = barcode.Split("\r\n")[2];
            }

        }

        [Fact]
        public void TestObjectoXml() 
        {
            TestXml testXml = new TestXml
            {
                Name = "Tom",
                Country = "China",
                Item = new List<Item>
                    {
                        new Item
                        {
                            TagCode = "top",
                            TagValue = "175",
                            TimeStamp = "1212212"
                        },
                        new Item
                        {
                            TagCode = "sex",
                            TagValue = "男",
                            TimeStamp = "1212212"
                        },
                        new Item
                        {
                            TagCode = "like",
                            TagValue = "music",
                            TimeStamp = "1212212"
                        },
                    }
            };
            string a = testXml.XMLSerialize();

            var data = a.DeXmlSerialize<TestXml>();
        }
    }

    [XmlRoot("StudentModel"), XmlType("StudentModel")]
    public class StudentModel
    {
        [XmlElement]
        public int Id { get; set; }

        [XmlElement]
        public string No { get; set; }
    }

    [XmlRoot("CallAgv")]
    public class TestXml
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlAttribute()]
        public string Country { get; set; }

        [XmlElement("item")]
        public List<Item> Item { get; set; }
    }

    public class Item
    {
        [XmlAttribute("tagCode")]
        public string TagCode { get; set; }

        [XmlAttribute("tagValue")]
        public string TagValue { get; set; }

        [XmlAttribute("timeStamp")]
        public string TimeStamp { get; set; }
    }

    public class Goods
    {
        public int Age { get; init; }
    }

    public class Person
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public int Top { get; set; }
    }

    public interface IRepo<T> where T : class
    {
        T Get();
    }

    public abstract class RepoBase<T> : IRepo<T> where T : class, new()
    {
        public RepoBase()
        {

        }

        public virtual T Get()
        {
            return new T();
        }
    }

    public class Repo<T> : RepoBase<T> where T : class, new()
    {
        public override T Get()
        {
            return new T();
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

    public class Configs
    {
        public string Name { get; set; } = "Bill";
    }

    public interface ICheck
    {

    }

    public class Check : ICheck
    {

    }

    public interface IM
    {
        int Age { get; set; }
    }

    public class M : IM
    {
        private ICheck check;
        public M(ICheck check)
        {
            this.check = check;
        }

        public int Age { get; set; } = 100;
    }

    public class MM : IM
    {
        public int Age { get; set; } = 123;
    }
}
