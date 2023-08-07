using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Xunit;
using Microsoft.Extensions.Logging;
using Ars.Common.Tool.Loggers;
using Ars.Common.Tool.Extension;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace ArsTest.ArsTests
{
    public class ArsLogTest
    {
        private readonly IServiceProvider serviceProvider;
        public ArsLogTest()
        {
            IHost host = Host.CreateDefaultBuilder()
             .ConfigureAppConfiguration(builder =>
             {

             })
             .ConfigureServices((builder, service) =>
             {
                 service.AddLogging();
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

            public ClassData()
            {
                CreationTime = DateTime.Now;
                this.Data = new object();
            }
        }
    }
}
