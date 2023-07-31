using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Xunit;
using Microsoft.Extensions.Logging;
using Ars.Common.Tool.Loggers;
using Ars.Common.Tool.Extension;
using Newtonsoft.Json;

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
                 //builder.AddJsonFile("config.json", true, false);
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

        [Fact]
        public void Test2() 
        {
            var a = JsonConvert.SerializeObject(123);
        }
    }
}
