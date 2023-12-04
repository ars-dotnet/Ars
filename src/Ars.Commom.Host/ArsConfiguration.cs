using Ars.Commom.Core;
using Ars.Common.Core;
using Ars.Common.Core.Configs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Host
{
    internal class ArsConfiguration : IArsConfiguration
    {
        public IList<IArsServiceExtension> ArsServiceExtensions { get; }

        public IList<IArsAppExtension> ArsAppExtensions { get; }

        /// <summary>
        /// Basic config
        /// </summary>
        public IArsBasicConfiguration? ArsBasicConfiguration { get; set; } = default;

        public IUnitOfWorkDefaultConfiguration? ArsUnitOfWorkDefaultConfiguration { get; set; } = default;

        /// <summary>
        /// ConsulDiscover config
        /// </summary>
        public IConsulDiscoverConfiguration? ArsConsulDiscoverConfiguration { get; set; } = default;

        /// <summary>
        /// ConsulRegister config
        /// </summary>
        public IConsulRegisterConfiguration? ArsConsulRegisterConfiguration { get; set; } = default;

        /// <summary>
        /// IdentityServer config
        /// </summary>
        public IArsIdentityServerConfiguration? ArsIdentityServerConfiguration { get; set; } = default;

        /// <summary>
        ///IdentityClient config
        /// </summary>
        public IArsIdentityClientConfiguration? ArsIdentityClientConfiguration { get; set; } = default;

        /// <summary>
        /// SingleDbContext config
        /// </summary>
        public IArsDbContextConfiguration? ArsDbContextConfiguration { get; set; } = default;

        /// <summary>
        /// MultupleDbContext config
        /// </summary>
        public IArsMultipleDbContextConfiguration? ArsMultipleDbContextConfiguration { get; set; } = default;

        /// <summary>
        /// Localization config
        /// </summary>
        public IArsLocalizationConfiguration? ArsLocalizationConfiguration { get; set; } = default;

        /// <summary>
        /// Redis config
        /// </summary>
        public IArsRedisConfiguration? ArsRedisConfiguration { get; set; } = default;

        /// <summary>
        /// Ocelot config
        /// </summary>
        public IArsOcelotConfiguration? ArsOcelotConfiguration { get; set; } = default;

        /// <summary>
        /// 
        /// </summary>
        public ArsConfiguration()
        {
            ArsServiceExtensions = new List<IArsServiceExtension>(0);
            ArsAppExtensions = new List<IArsAppExtension>(0);
        }

        public void AddArsServiceExtension(IArsServiceExtension optExtension)
        {
            ArsServiceExtensions.Add(optExtension);
        }

        public void AddArsAppExtension(IArsAppExtension appExtension)
        {
            ArsAppExtensions.Add(appExtension);
        }
    }
}
