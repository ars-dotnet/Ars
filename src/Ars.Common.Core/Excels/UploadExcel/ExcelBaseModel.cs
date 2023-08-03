using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Excels.UploadExcel
{
    public abstract class ExcelBaseModel : IExcelModel
    {
        public bool IsErr { get; set; }

        [ExcelMapping("错误汇总", ReadOrWrite.Write)]
        public IDictionary<string, string>? FieldErrMsg { get; set; }
    }
}
