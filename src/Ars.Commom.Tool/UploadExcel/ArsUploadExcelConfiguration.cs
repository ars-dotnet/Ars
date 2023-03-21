using Ars.Common.Tool.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.UploadExcel
{
    internal class ArsUploadExcelConfiguration : IArsUploadExcelConfiguration
    {
        public string UploadRoot { get; set; }

        public string RequestPath { get; set; }

        public TimeSpan SlidingExpireTime { get; set; } = TimeSpan.FromDays(7);
    }
}
