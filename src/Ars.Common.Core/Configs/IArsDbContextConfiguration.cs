using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Configs
{
    public interface IArsDbContextConfiguration
    {
        string DefaultString { get; set; }

        /// <summary>
        /// 1 mysql
        /// 2 mssql
        /// </summary>
        int? DbType { get; set; }

        bool UseLazyLoadingProxies { get; set; }
    }
}
