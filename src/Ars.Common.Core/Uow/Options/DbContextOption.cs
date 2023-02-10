using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Uow.Options
{
    public class DbContextOption
    {
        public string DefaultString { get; set; }

        public int? DbType { get; set; }

        public bool UseLazyLoadingProxies { get; set; } = true;
    }
}
