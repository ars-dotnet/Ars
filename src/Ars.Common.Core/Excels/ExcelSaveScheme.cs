using Ars.Common.Core.Excels.ExportExcel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Excels
{
    public class ExcelSaveScheme : ExcelExportScheme
    {
        public string SavePath { get; set; }

        public TimeSpan SlidingExpireTime { get; set; }
    }
}
