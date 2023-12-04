using Ars.Commom.Core;
using Ars.Common.Core.Configs;
using Ars.Common.Core.Uow.Options;
using Ars.Common.EFCore.EfCoreUnitOfWorks;
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
        /// <summary>
        /// 添加单一数据库上下文
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <param name="arsServiceBuilder"></param>
        /// <param name="optAction"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static IArsWebApplicationBuilder AddArsDbContext<TDbContext>(
            this IArsWebApplicationBuilder arsServiceBuilder,
            Action<DbContextOptionsBuilder>? optAction = null)
            where TDbContext : ArsDbContext
        {
            var option =
                arsServiceBuilder.Configuration
                .GetSection(nameof(ArsDbContextConfiguration))
                .Get<ArsDbContextConfiguration>() 
                ?? 
                throw new ArgumentNullException("appsetting => ArsDbContextConfiguration not be null!");

            if (null != arsServiceBuilder.ArsConfiguration.ArsDbContextConfiguration) 
            {
                throw new ArgumentNullException("AddArsDbContext just called once;Call AddMultipleArsDbContext to registe multiple dbcontext if you want");
            }

            arsServiceBuilder.ArsConfiguration.ArsDbContextConfiguration = option;

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

            var service = arsServiceBuilder.Services;
            service.AddSingleton<IArsDbContextConfiguration>(_ => option);

            //dbcontext注册为瞬时
            service.AddDbContextFactory<TDbContext>(optAction, lifetime: ServiceLifetime.Transient);

            return arsServiceBuilder;
        }

        /// <summary>
        /// 添加多数据库上下文
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <param name="arsServiceBuilder"></param>
        /// <param name="optAction"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static IArsWebApplicationBuilder AddMultipleArsDbContext<TDbContext>(
           this IArsWebApplicationBuilder arsServiceBuilder,
           Action<IServiceProvider,DbContextOptionsBuilder>? optAction = null)
           where TDbContext : ArsDbContext
        {
            var service = arsServiceBuilder.Services;
            var options =
                arsServiceBuilder.Configuration
                .GetSection(nameof(ArsMultipleDbContextConfiguration))
                .Get<ArsMultipleDbContextConfiguration>()
                ?? 
                throw new ArgumentNullException("appsetting => ArsMultipleDbContextConfiguration not be null!");

            if ((!options.ArsDbContextConfigurations?.Any(r => r.DbContextFullName.Equals(typeof(TDbContext).FullName))) ?? false)
            {
                throw new ArgumentNullException($"appsetting => ArsMultipleDbContextConfiguration has no DbContextFullName equals '{typeof(TDbContext).FullName}'");
            }

            if (null == arsServiceBuilder.ArsConfiguration.ArsMultipleDbContextConfiguration) 
            {
                arsServiceBuilder.ArsConfiguration.ArsMultipleDbContextConfiguration = options;

                service.AddSingleton<IArsMultipleDbContextConfiguration>(_ =>options);
            };

            var option = options.ArsDbContextConfigurations!
                .FirstOrDefault(r => r.DbContextFullName.Equals(typeof(TDbContext).FullName))!;
            if (null == optAction)
            {
                switch (option.DbType)
                {
                    case 1:
                        optAction = (sp,bulider) =>
                        {
                            var strategy = sp.GetService<IEFCoreExistTransactionConnectionStorage>();
                            if (strategy?.TryGetConnection(option.DefaultString, out var connection) ?? false)
                            {
                                bulider.UseMySql(connection!, ServerVersion.AutoDetect(option.DefaultString));
                            }
                            else 
                            {
                                bulider.UseMySql(option.DefaultString, ServerVersion.AutoDetect(option.DefaultString));
                            }
                        };
                        break;
                    case 2:
                        optAction = (sp,bulider) =>
                        {
                            var strategy = sp.GetService<IEFCoreExistTransactionConnectionStorage>();
                            if (strategy?.TryGetConnection(option.DefaultString, out var connection) ?? false) 
                            {
                                bulider.UseSqlServer(connection!);
                            }
                            else 
                            {
                                bulider.UseSqlServer(option.DefaultString);
                            }
                        };
                        break;
                    default: throw new ArgumentException("暂时只支持mysql和mssql数据库");
                }
            }
            //dbcontext注册为瞬时
            service.AddDbContextFactory<TDbContext>(optAction,lifetime: ServiceLifetime.Transient);

            return arsServiceBuilder;
        }
    }
}
