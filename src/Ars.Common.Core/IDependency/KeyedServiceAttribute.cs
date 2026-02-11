using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.IDependency
{
    [AttributeUsage(AttributeTargets.Class)]
    public class KeyedServiceAttribute : Attribute
    {
        public string ServiceName { get; }

        public KeyedServiceAttribute(string serviceName)
        {
            ServiceName = serviceName;
        }
    }
}
