using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Excels.UploadExcel.Validation
{
    public class ExcelNumberAttribute : ExcelRangeAttribute
    {
        public ExcelNumberAttribute() : base(-9999999999.0, 9999999999.0)
        {

        }
    }
}
