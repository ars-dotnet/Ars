using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ars.Commom.Tool.Extension
{
    public static class IEnumerableExtension
    {
        public static bool HasValue<T>(this IEnumerable<T>? ts) 
        {
            return ts?.Any() ?? false;
        }
    }
}
