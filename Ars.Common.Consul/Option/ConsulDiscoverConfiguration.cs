using Ars.Common.Core.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Consul.Option
{
    public class ConsulDiscoverConfiguration : IConsulDiscoverConfiguration
    {
        public IEnumerable<ConsulConfiguration> ConsulDiscovers { get; set; }
    }
}
