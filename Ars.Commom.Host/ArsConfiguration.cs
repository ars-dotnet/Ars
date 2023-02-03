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
        public IList<IArsOptExtension> ArsOptExtensions { get; }

        public IUnitOfWorkDefaultConfiguration? UnitOfWorkDefaultConfiguration { get; set; } = default;

        public IConsulDiscoverConfiguration? ConsulDiscoverConfiguration { get; set; } = default;

        public IConsulRegisterConfiguration? ConsulRegisterConfiguration { get; set; } = default;

        public IArsIdentityServerConfiguration? ArsIdentityServerConfiguration { get; set; } = default;

        public IArsIdentityClientConfiguration? ArsIdentityClientConfiguration { get; set; } = default;

        public bool AddArsIdentityServerClient { get; set; }

        public ArsConfiguration()
        {
            ArsOptExtensions = new List<IArsOptExtension>(0);
        }

        public void AddArsOptExtension(IArsOptExtension optExtension)
        {
            ArsOptExtensions.Add(optExtension);
        }
    }
}
