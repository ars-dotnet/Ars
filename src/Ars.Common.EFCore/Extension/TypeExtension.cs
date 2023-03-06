using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.EFCore.Extension
{
    public static class TypeExtension
    {
        public static bool IsArsDbContextType(this Type type)
        {
            return type.IsClass && !type.IsAbstract && !type.IsInterface && typeof(ArsDbContext).IsAssignableFrom(type);
        }
    }
}
