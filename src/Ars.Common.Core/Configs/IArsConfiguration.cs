using Ars.Common.Core.IDependency;
using Ars.Common.Core.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Configs
{
    public interface IArsConfiguration
    {
        //服务配置集合
        IList<IArsServiceExtension> ArsServiceExtensions { get; }

        //添加服务配置集合
        void AddArsServiceExtension(IArsServiceExtension optExtension);

        //中间件集合
        IList<IArsAppExtension> ArsAppExtensions { get; }

        //添加中间件到集合
        void AddArsAppExtension(IArsAppExtension appExtension);

        /// <summary>
        /// 基础配置
        /// </summary>
        IArsBasicConfiguration? ArsBasicConfiguration { get; set; }

        /// <summary>
        /// efcore transaction
        /// </summary>
        IUnitOfWorkDefaultConfiguration? ArsUnitOfWorkDefaultConfiguration { get; set; }

        /// <summary>
        /// consul discover config
        /// </summary>
        IConsulDiscoverConfiguration? ArsConsulDiscoverConfiguration { get; set; }

        /// <summary>
        /// consul register config
        /// </summary>
        IConsulRegisterConfiguration? ArsConsulRegisterConfiguration { get; set; }

        /// <summary>
        /// IdentityServer4 server config
        /// </summary>
        IArsIdentityServerConfiguration? ArsIdentityServerConfiguration { get; set; }

        /// <summary>
        /// IdentityServer4 client config
        /// </summary>
        IArsIdentityClientConfiguration? ArsIdentityClientConfiguration { get; set; }

        /// <summary>
        /// EfCore Dbcontext config
        /// </summary>
        IArsDbContextConfiguration? ArsDbContextConfiguration { get; set; }

        /// <summary>
        /// Localization config
        /// </summary>
        IArsLocalizationConfiguration? ArsLocalizationConfiguration { get; set; }

        /// <summary>
        /// Redis config
        /// </summary>
        IArsRedisConfiguration? ArsRedisConfiguration { get; set; }
    }
}
