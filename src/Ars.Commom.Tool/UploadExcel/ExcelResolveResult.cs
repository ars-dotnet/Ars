using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.UploadExcel
{
    public class ExcelResolveResult
    {
        public Dictionary<string, string> Column { get; set; }

        public Type ItemType { get; set; }

        public IEnumerable List { get; set; }
    }
}
