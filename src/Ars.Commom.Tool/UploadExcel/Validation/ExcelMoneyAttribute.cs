using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.UploadExcel.Validation
{
    public class ExcelMoneyAttribute : ExcelRangeAttribute
    {
        public ExcelMoneyAttribute() : base(-9999999999.0, 9999999999.0)
        {

        }
    }
}
