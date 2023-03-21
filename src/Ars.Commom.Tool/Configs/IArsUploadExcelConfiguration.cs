using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.Configs
{
    /// <summary>
    /// static files configuration
    /// </summary>
    public interface IArsUploadExcelConfiguration
    {
        public string UploadRoot { get; set; }

        public string RequestPath { get; set; }

        public TimeSpan SlidingExpireTime { get; set; }
    }
}
