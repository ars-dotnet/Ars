using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Consul.Option
{
    public class ConsulRegisteOption
    {
        public string ConsulAddress { get; set; }

        public string ServiceName { get; set; }

        public string ServiceIp { get; set; }

        public int ServicePort { get; set; }
    }
}
