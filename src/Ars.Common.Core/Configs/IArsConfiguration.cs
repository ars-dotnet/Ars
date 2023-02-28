using Ars.Common.Core.IDependency;
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
        /// efcore transaction
        /// </summary>
        public IUnitOfWorkDefaultConfiguration? UnitOfWorkDefaultConfiguration { get; set; }

        /// <summary>
        /// consul discover config
        /// </summary>
        public IConsulDiscoverConfiguration? ConsulDiscoverConfiguration { get; set; }

        /// <summary>
        /// consul register config
        /// </summary>
        public IConsulRegisterConfiguration? ConsulRegisterConfiguration { get; set; }

        /// <summary>
        /// IdentityServer4 server config
        /// </summary>
        public IArsIdentityServerConfiguration? ArsIdentityServerConfiguration { get; set; }

        /// <summary>
        /// IdentityServer4 client config
        /// </summary>
        public IArsIdentityClientConfiguration? ArsIdentityClientConfiguration { get; set; }

        /// <summary>
        /// EfCore Dbcontext config
        /// </summary>
        public IArsDbContextConfiguration? ArsDbContextConfiguration { get; set; }

        /// <summary>
        /// Localization config
        /// </summary>
        public IArsLocalizationConfiguration? ArsLocalizationConfiguration { get; set; }

        /// <summary>
        /// Redis config
        /// </summary>
        public IArsRedisConfiguration? ArsRedisConfiguration { get; set; }
    }
}
