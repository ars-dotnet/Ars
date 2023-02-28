using Ars.Common.Core.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Uow.Options
{
    public class ArsDbContextConfiguration : IArsDbContextConfiguration
    {
        public string DefaultString { get; set; }

        public int? DbType { get; set; }

        public bool UseLazyLoadingProxies { get; set; } = true;
    }
}
