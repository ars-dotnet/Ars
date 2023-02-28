using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.Extension
{
    public static class TypeExtension
    {
        public static bool IsAssignableGenericFrom(this Type baseType, Type type)
        {
            if (type.GetInterfaces().Any(r => r.IsGenericType && r.GetGenericTypeDefinition() == baseType))
                return true;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == baseType)
                return true;

            Type? bType = type.BaseType;
            if (bType == null) return false;
            return baseType.IsAssignableGenericFrom(bType);
        }
    }
}
