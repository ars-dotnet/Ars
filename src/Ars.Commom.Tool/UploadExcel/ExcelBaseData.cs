using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.UploadExcel
{
    public abstract class ExcelBaseData<T> : IExcelData<T>
        where T : IExcelModel
    {
        public IFormFile File { get; set; }

        public IList<T> ExcelModels { get; set; }
    }
}
