using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.AutoFac
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AutowiredAttribute : Attribute
    {
        public string ServiceName { get; }

        public AutowiredAttribute(string serviceName = null)
        {
            ServiceName = serviceName;
        }
    }
}
