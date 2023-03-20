using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.UploadExcel
{
    public interface IExcelData<T>
        where T : IExcelModel
    {
        IFormFile File { get; set; }

        IList<T> ExcelModels { get; set; }
    }
}
