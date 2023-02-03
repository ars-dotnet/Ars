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
            Action<DbContextOption>? action = null, 
            Action<DbContextOptionsBuilder>? optAction = null)
            where TDbContext : ArsDbContext
        {
            DbContextOption option = new();
            action?.Invoke(option);

            var service = arsServiceBuilder.Services;
            IConfiguration configuration = service.Provider.GetRequiredService<IConfiguration>();
            if (string.IsNullOrEmpty(option.DefaultString))
            {
                option.DefaultString = configuration.GetSection("DefaultString").Get<string>();
                if (string.IsNullOrEmpty(option.DefaultString))
                    throw new ArgumentNullException(nameof(option.DefaultString));
            }

            option.DbType = configuration.GetSection("DbType").Get<int>();
            service.ServiceCollection
                .AddSingleton<IOptions<DbContextOption>>(new OptionsWrapper<DbContextOption>(option));

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
                    default: throw new ArgumentException("Configuration => DbType is null");
                }
            }
            service.ServiceCollection.AddDbContextFactory<TDbContext>(optAction);
            
            return arsServiceBuilder;
        }
    }
}
