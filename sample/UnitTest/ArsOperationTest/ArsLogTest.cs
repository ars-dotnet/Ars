using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Xunit;
using Microsoft.Extensions.Logging;
using Ars.Common.Tool.Loggers;
using Ars.Common.Tool.Extension;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Reflection;
using ArsOperationTest;
using System.Globalization;
using System.Runtime.ExceptionServices;

namespace ArsTest.ArsTests
{
    public class ArsLogTest
    {
        private readonly IServiceProvider serviceProvider;
        private static ClassData classData = new ClassData();
        public ArsLogTest()
        {
            IHost host = Host.CreateDefaultBuilder()
             .ConfigureAppConfiguration(builder =>
             {

             })
             .ConfigureServices((builder, service) =>
             {
                 service.AddLogging();

                 service.AddScoped(_ => new MyService());
                 service.AddSingleton(_ => new MyService());
                 service.AddTransient(_ => new MyService());
             }).
             ConfigureLogging((hostingContext, logging) =>
             {
                 logging.AddLog4Net("Configs/log4net.Config");
                 logging.AddArsLog4Net("Configs/arslog4net.Config");
             })
             .Build();

            serviceProvider = host.Services;

            
        }

        /// <summary>
        /// 测试扩展的log4net,到达不同categoryName记录不同的日志文本
        /// </summary>
        [Fact]
        public void Test1()
        {
            var loggerFac = serviceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFac.CreateLogger($"{ArsLogNames.CustomLogCategoryPrefix}.MASS.01.A7");
            logger.LogInformation("good 产线01.站台A7.业务:组合台是否空闲可下任务 开启轮询监听");
            logger.LogError("bad 产线01.站台A7.业务:组合台是否空闲可下任务 开启轮询监听");

            logger = loggerFac.CreateLogger($"{ArsLogNames.CustomLogCategoryPrefix}.MASS.01.A7");
            logger.LogInformation("good1 产线01.站台A7.业务:组合台是否空闲可下任务 开启轮询监听");
            logger.LogError("bad1 产线01.站台A7.业务:组合台是否空闲可下任务 开启轮询监听");

            logger = loggerFac.CreateLogger($"{ArsLogNames.CustomLogCategoryPrefix}.MASS.01.A8");
            logger.LogInformation("good2 产线01.站台A8.业务:组合台是否空闲可下任务 开启轮询监听");
            logger.LogError("bad2 产线01.站台A8.业务:组合台是否空闲可下任务 开启轮询监听");
        }

        /// <summary>
        /// JsonConvert值类型序列化不会抛异常
        /// </summary>
        [Fact]
        public void Test2()
        {
            var a = JsonConvert.SerializeObject(123);

            ExceptionDispatchInfo.Capture(new Exception()).Throw();
        }

        public class BaseAnimal 
        {
            public static int count = 0;
        }


        public class Cat : BaseAnimal
        {
            public void Add() 
            {
                count++;
            }
        }

        public class Dog : BaseAnimal 
        {
            public void Add()
            {
                count ++;
            }
        }

        [Fact]
        public void TestBaseStatic() 
        {
            Cat cat = new Cat();
            Dog dog = new Dog();

            cat.Add();
            dog.Add();
        }

        /// <summary>
        /// task并发，某个task抛异常，不会影响其它task执行
        /// </summary>
        [Fact]
        public async void Test3()
        {
            Task[] tasks = new Task[]
            {
                Task.Run(() =>
                {
                    int.Parse("xxx");
                }),

                Task.Run(() =>
                {
                    int.Parse("yyy");
                }),

                Task.Run(() =>
                {
                    int.Parse("123");
                }),
              };

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// test ConditionalWeakTable
        /// 当key被释放后，ConditionalWeakTable会自动清除掉key相关的数据
        /// </summary>
        [Fact]
        public void Test4()
        {
            var mc1 = new ManagedClass();
            var mc2 = new ManagedClass();
            var mc3 = new ManagedClass();

            var cwt = new ConditionalWeakTable<ManagedClass, ClassData>
            {
                { mc1, new ClassData() },
                { mc2, new ClassData() },
                { mc3, new ClassData() }
            };

            var wr2 = new WeakReference(mc2);
            mc2 = null;

            GC.Collect();

            int a;
            if (wr2.Target == null)
                a = 0;
            else if (cwt.TryGetValue((ManagedClass)wr2.Target, out ClassData? data))
                a = 1;
            else
                a = 2;

            Assert.True(a == 0);
        }

        public class ManagedClass
        {
        }

        public class ClassData
        {
            public DateTime CreationTime;
            public object Data;

            public static int Age;
            public int Top;

            public ClassData()
            {
                CreationTime = DateTime.Now;
                this.Data = new object();
            }

            public ClassData(int top) 
            {
                Top = top;
            }

            public int GetAge() 
            {
                return Age;
            }
        }

        /// <summary>
        /// 测试& |
        /// 枚举值为2的指数倍
        /// 转化为2进制来运算
        /// </summary>
        [Fact]
        public void Test5()
        {
            // &
            Types p1 = Types.First & Types.None;
            Types p2 = Types.First & Types.None & Types.Second;
            Types p3 = Types.First & Types.Second;
            Assert.True(p1 == Types.None);
            Assert.True(p2 == Types.None);
            Assert.True(p3 == Types.None);

            // | 
            Types p11 = Types.None | Types.First;
            Types p12 = Types.None | Types.First | Types.Second | Types.Fiveth;
            Assert.True(p11 == Types.First);
            Assert.False(p12 == (Types.First | Types.Second));
            Assert.True(p12 == (Types.First | Types.Second | Types.Fiveth));

            // | - &
            Types p21 = Types.First & p11;
            Types p22 = Types.Fiveth & p12;
            Assert.True(p21 == Types.First);
            Assert.True(p22 == Types.Fiveth);

            Types p23 = Types.None | Types.First | Types.Second | Types.Sixth;
            Assert.True((p12 & p23) == (Types.First | Types.Second));
        }

        [Flags]
        public enum Types 
        {
            None = 0,

            First = 2,

            Second = 4,

            Third = 8,

            /// <summary>
            /// 1*16^1 + 0*16*0 = 16
            /// </summary>
            Fourth = 0x10,

            /// <summary>
            /// 2*16^1 + 0*16*0 = 32
            /// </summary>
            Fiveth = 0x20,

            Sixth = 0x40,

            Seventh = 0x80,
        }

        /// <summary>
        /// 测试全局静态变量多次初始化
        /// </summary>
        [Fact]
        public void Test6() 
        {
            ClassData.Age = 66;

            classData.Data = 123;
            var a = classData.GetHashCode();
            var age = classData.GetAge();

            classData = new ClassData();
            var b = classData.GetHashCode();
            var age1 = classData.GetAge();

            Assert.False(a == b);
            Assert.True(age == age1 && age == 66);
        }

        /// <summary>
        /// 测试task foreach
        /// </summary>
        [Fact]
        public async void Test7() 
        {
            IList<ClassData> classDatas = new List<ClassData>()
            {
                new ClassData(12),
                new ClassData(22),
                new ClassData(33),
            };

            List<Task<int>> tasks = new List<Task<int>>();
            foreach (var data in classDatas)
            {
                tasks.Add(Task.Run(() =>
                {
                    var d = data;
                    return d.Top;
                }));
            }

            var mmm = await Task.WhenAll(tasks);
        }

        public class Studentss 
        {
            public int? Id { get; set; }

            public Toys? Toys { get; set; }

            public Toys Toyss { get; set; }
        }

        public class Toys{ }

        [Fact]
        public void Test8() 
        {
            var type1 = typeof(Studentss).GetProperty("Id")!.PropertyType;
            Assert.True(type1.IsGenericType);
            Assert.True(type1.GetGenericTypeDefinition() == typeof(Nullable<>));

            var type = typeof(Studentss).GetProperty("Toys")!.PropertyType;
            Assert.False(type.IsGenericType);

            NullabilityInfoContext context = new();
            NullabilityInfo arrayInfo = context.Create(typeof(Studentss).GetProperty("Toys")!);
            NullabilityInfo arrayInfo1 = context.Create(typeof(Studentss).GetProperty("Toyss")!);
        }

        [Fact]
        public void Test9()
        {
            IEnumerable<string> a = new List<string>
            {
                "jerry;bill",
                "tom"
            };

            var atr = string.Join(";",a);

            Assert.True(atr.IndexOf("jerry") >= 0);
            Assert.True(atr.IndexOf("bill") >= 0);

            string m = null;
            Assert.True(m.IndexOf("ad") < 0);
        }

        [Fact]
        public void TestDateTime() 
        {
            var a = new DateTime(2023, 3, 10, 10, 23, 30);

            DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();

            dtFormat.ShortDatePattern = "yyyy-MM-dd HH:mm:ss";
            var m = Convert.ToDateTime("2023-03-10 10:06:35.345", dtFormat);
            var specified = DateTime.SpecifyKind(m, DateTimeKind.Local);

            var b = DateTime.Now;
        }

    }
}
