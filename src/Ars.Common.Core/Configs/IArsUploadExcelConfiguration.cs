using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Configs
{
    /// <summary>
    /// static files configuration
    /// </summary>
    public interface IArsUploadExcelConfiguration
    {
        /// <summary>
        /// 文件存储位置
        /// </summary>
        public string UploadRoot { get; set; }

        /// <summary>
        /// 文件请求地址
        /// </summary>
        public string RequestPath { get; set; }

        /// <summary>
        /// 文件保存时间
        /// </summary>
        public TimeSpan SlidingExpireTime { get; set; }
    }
}
