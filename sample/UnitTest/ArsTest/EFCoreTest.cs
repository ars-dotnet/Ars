using Ars.Commom.Host.Extension;
using Ars.Common.Core.Uow;
using Ars.Common.EFCore.Extension;
using Ars.Common.EFCore.Repository;
using ArsWebApiService;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyApiWithIdentityServer4.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace ArsTest
{
    public class EFCoreTest
    {
        private IServiceProvider _serviceProvider;

        public EFCoreTest()
        {
            var builder = WebApplication.CreateBuilder();

            builder.Host.ConfigureHostConfiguration(builder =>
            {
                builder.AddJsonFile("appsettings.json", true, false);
            });

            builder.Services
                .AddArserviceCore(builder, config =>
                {
                    //多数据源 mysql
                    config.AddArsMultipleDbContext<MyDbContext>();
                });

            builder.Logging.AddLog4Net("Configs/log4net.Config");
            var app = builder.Build();
            app.UseArsCore();

            _serviceProvider = app.Services;
        }

        [Fact]
        public async Task Test() 
        {
            var unitofwork = _serviceProvider.GetRequiredService<IUnitOfWorkManager>();

            using var unitofworkscope = unitofwork.Begin(TransactionScopeOption.RequiresNew);

            var service = _serviceProvider.GetRequiredService<IRepository<Student, Guid>>();
            try
            {
                var data = await service.FirstOrDefaultAsync();

                data.FirstMidName = "123456";

                await unitofwork.Current.SaveChangesAsync();
            }
            catch (Exception e)
            {

            }

            await unitofworkscope.CompleteAsync();
        }
    }
}
