using Ars.Commom.Core;
using Ars.Common.Core;
using Ars.Common.Core.Configs;
using Ars.Common.Tool.Configs;
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
        public string Root { get; private set; }

        public string ApplicationUrl { get; set; }

        public IList<IArsServiceExtension> ArsServiceExtensions { get; }

        public IList<IArsAppExtension> ArsAppExtensions { get; }

        public IUnitOfWorkDefaultConfiguration? UnitOfWorkDefaultConfiguration { get; set; } = default;

        public IConsulDiscoverConfiguration? ConsulDiscoverConfiguration { get; set; } = default;

        public IConsulRegisterConfiguration? ConsulRegisterConfiguration { get; set; } = default;

        public IArsIdentityServerConfiguration? ArsIdentityServerConfiguration { get; set; } = default;

        public IArsIdentityClientConfiguration? ArsIdentityClientConfiguration { get; set; } = default;

        public IArsDbContextConfiguration? ArsDbContextConfiguration { get; set; } = default;
        
        public IArsLocalizationConfiguration? ArsLocalizationConfiguration { get; set; } = default;

        /// <summary>
        /// Redis config
        /// </summary>
        public IArsRedisConfiguration? ArsRedisConfiguration { get; set; } = default;

        public ArsConfiguration()
        {
            Root = AppDomain.CurrentDomain.BaseDirectory;
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
