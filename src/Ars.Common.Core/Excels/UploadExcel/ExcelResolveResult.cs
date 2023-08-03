using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Excels.UploadExcel
{
    public class ExcelResolveResult
    {
        public IEnumerable<ExcelColumn> Column { get; set; }

        public Type ItemType { get; set; }

        public IEnumerable List { get; set; }
    }
}
