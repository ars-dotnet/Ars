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
        //扩展集合
        IList<IArsOptExtension> ArsOptExtensions { get; }

        //添加配置扩展到集合
        void AddArsOptExtension(IArsOptExtension optExtension);

        /// <summary>
        /// efcore事务
        /// </summary>
        public IUnitOfWorkDefaultConfiguration? UnitOfWorkDefaultConfiguration { get; set; }

        /// <summary>
        /// consul discover
        /// </summary>
        public IConsulDiscoverConfiguration? ConsulDiscoverConfiguration { get; set; }

        /// <summary>
        /// consul register
        /// </summary>
        public IConsulRegisterConfiguration? ConsulRegisterConfiguration { get; set; }

        /// <summary>
        /// IdentityServer4 server config
        /// </summary>
        public IArsIdentityServerConfiguration? ArsIdentityServerConfiguration { get; set; }

        /// <summary>
        /// add IdentityServer4 client
        /// </summary>
        public IArsIdentityClientConfiguration? ArsIdentityClientConfiguration { get; set; }
    }
}
