using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.AutoFac.Helper
{
    public interface IInjectPropertySelector
    {
        bool TryGetInjectProperty(PropertyInfo propertyInfo, object instance, out string serviceName);
    }
}
