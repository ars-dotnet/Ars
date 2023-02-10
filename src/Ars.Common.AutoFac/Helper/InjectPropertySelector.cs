using Ars.Common.AutoFac.IDependency;
using Ars.Common.Core.IDependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.AutoFac.Helper
{
    internal class InjectPropertySelector : IInjectPropertySelector
    {
        public bool TryGetInjectProperty(PropertyInfo propertyInfo, object instance, out string serviceName)
        {
            var inject = propertyInfo.GetCustomAttributes().OfType<AutowiredAttribute>().FirstOrDefault();
            serviceName = inject?.ServiceName;

            return inject != null;
        }
    }
}
