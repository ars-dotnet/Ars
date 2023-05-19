using Ars.Commom.Core;
using Ars.Common.Core.Configs;
using Ars.Common.Core.Uow.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.EFCore.Extension
{
    public static class IServiceCollectionExtension
    {
        public static IArsServiceBuilder AddArsDbContext<TDbContext>(
            this IArsServiceBuilder arsServiceBuilder,
            Action<DbContextOptionsBuilder>? optAction = null)
            where TDbContext : ArsDbContext
        {
            var service = arsServiceBuilder.Services;
            var option =
                arsServiceBuilder.Configuration
                .GetSection(nameof(ArsDbContextConfiguration))
                .Get<ArsDbContextConfiguration>() 
             ?? throw new Exception("appsetting => ArsDbContextConfiguration not be null!");

            service.AddSingleton<IArsDbContextConfiguration>(option);
            arsServiceBuilder.ServiceProvider.GetRequiredService<IArsConfiguration>().ArsDbContextConfiguration ??= option;

            if (null == optAction)
            {
                switch (option.DbType)
                {
                    case 1:
                        optAction = bulider => bulider.UseMySql(option.DefaultString, ServerVersion.AutoDetect(option.DefaultString));
                        break;
                    case 2:
                        optAction = bulider => bulider.UseSqlServer(option.DefaultString);
                        break;
                    default: throw new ArgumentException("暂时只支持mysql和mssql数据库");
                }
            }
            service.AddDbContextFactory<TDbContext>(optAction);
            
            return arsServiceBuilder;
        }
    }
}
