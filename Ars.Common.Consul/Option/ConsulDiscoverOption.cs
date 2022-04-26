using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Consul.Option
{
    public class ConsulDiscoverOption
    {
        public IEnumerable<ConsulDiscover> ConsulDiscovers { get; set; }
    }

    public class ConsulDiscover
    {
        public string ServiceName { get; set; }

        public string ConsulAddress { get; set; }
    }
}
