using Ars.Commom.Core;
using Ars.Common.EFCore.Options;
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
            IConfiguration configuration, 
            Action<DbContextOption>? action = null, 
            Action<DbContextOptionsBuilder>? optionsAction = null)
            where TDbContext : ArsDbContext
        {
            var services = arsServiceBuilder.Services.ServiceCollection;
            DbContextOption option = new DbContextOption();
            action?.Invoke(option);
            if (string.IsNullOrEmpty(option.DefaultString)) 
            {
                option.DefaultString = configuration.GetSection("DefaultString").Get<string>();
                if (string.IsNullOrEmpty(option.DefaultString))
                    throw new ArgumentNullException(nameof(option.DefaultString));
            }
            services.AddSingleton<IOptions<DbContextOption>>(new OptionsWrapper<DbContextOption>(option));

            if (null == optionsAction) 
            {
                optionsAction = bulider => bulider.UseMySql(option.DefaultString, ServerVersion.AutoDetect(option.DefaultString));
            }
            services.AddDbContextFactory<TDbContext>(optionsAction);

            return arsServiceBuilder;
        }
    }
}
