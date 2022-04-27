using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.EFCore.Options
{
    public class DbContextOption
    {
        public string DefaultString { get; set; }

        public bool UseLazyLoadingProxies { get; set; } = true;
    }
}
