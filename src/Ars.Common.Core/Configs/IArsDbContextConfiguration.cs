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

        /// <summary>
        /// 是否启用延迟加载
        /// 默认不启用
        /// </summary>
        bool UseLazyLoadingProxies { get; set; }

        /// <summary>
        /// dbcontext唯一
        /// </summary>
        string DbContextFullName { get; set; }
    }
}
