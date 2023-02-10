using Ars.Common.Core.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Consul.Option
{
    internal class ConsulRegisterConfiguration : IConsulRegisterConfiguration
    {
        public string ConsulAddress { get; set; }

        public string ServiceName { get; set; }

        public string ServiceIp { get; set; }

        public int ServicePort { get; set; }

        public string HttpHealthAction { get; set; }
    }
}
