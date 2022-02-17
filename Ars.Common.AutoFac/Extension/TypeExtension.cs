using Ars.Common.AutoFac.IDependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.AutoFac.Extension
{
    internal static class TypeExtension
    {
        public static bool IsArsModuleType(this Type type)
        {
            return type.IsClass && !type.IsAbstract && !type.IsInterface && typeof(ArsAutofacModule).IsAssignableFrom(type);
        }

        public static bool IsArsRegisterInterfaceType(this Type type)
        {
            return type.IsClass && !type.IsAbstract && !type.IsInterface && typeof(IArsDependency).IsAssignableFrom(type);
        }
    }
}
